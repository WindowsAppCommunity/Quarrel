/*

 Copyright (c) 2017, Robin Raymond
 All rights reserved.

 Redistribution and use in source and binary forms, with or without
 modification, are permitted provided that the following conditions are met:

 1. Redistributions of source code must retain the above copyright notice, this
 list of conditions and the following disclaimer.
 2. Redistributions in binary form must reproduce the above copyright notice,
 this list of conditions and the following disclaimer in the documentation
 and/or other materials provided with the distribution.

 THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

 The views and conclusions contained in the software and documentation are those
 of the authors and should not be interpreted as representing official policies,
 either expressed or implied, of the FreeBSD Project.
 
 */

#pragma once

#include <zsLib/types.h>
#include <zsLib/Exception.h>
#include <zsLib/SafeInt.h>
#include <zsLib/String.h>
#include <zsLib/XML.h>
#include <zsLib/Numeric.h>
#include <zsLib/Stringize.h>
#include <zsLib/ISettings.h>

#include <limits>
#include <list>
#include <map>

#ifdef max
#define ZSLIB_RANGE_UNDEFED_MAX
#undef max
#endif // max

namespace zsLib
{
  namespace internal
  {
    void throwRangeSelectionInvalidArgumentStartStopStr(const String &startStr, const String &endStr);
    void throwRangeSelectionBadStateIf(bool result, const char *str);
    void throwRangeSelectionBadState(const char *str);

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //
    // RangeSelection
    //

    template <
      typename RangeType, 
      typename LargeUnsignedType = unsigned long long,
      typename LargeSignedType = signed long long
    >
    class RangeSelection
    {
    public:
      typedef RangeType UseRangeType;
      typedef LargeUnsignedType UseLargeUnsignedType;
      typedef LargeSignedType UseLargeSignedType;

      typedef std::pair<UseRangeType, UseRangeType> RangeTypePair;
      typedef std::list<RangeTypePair> RangeTypeList;
      typedef std::map<UseRangeType, RangeTypePair> RangeMap;

      typedef zsLib::XML::Document Document;
      typedef zsLib::XML::Element Element;
      typedef zsLib::XML::Text Text;

    public:
      //-----------------------------------------------------------------------
      RangeSelection() noexcept {}

      //-----------------------------------------------------------------------
      RangeSelection(const RangeSelection &op2) noexcept :
        allows_(op2.allows_),
        denies_(op2.denies_)
      {
      }

      //-----------------------------------------------------------------------
      ~RangeSelection() {}

      //-----------------------------------------------------------------------
      RangeSelection &operator=(const RangeSelection &op2) noexcept
      {
        if (this == &(op2)) return *this;
        allows_ = op2.allows_;
        denies_ = op2.denies_;
        dirty_ = true;
        return *this;
      }

      //-----------------------------------------------------------------------
      void importFromSetting(const char *key) noexcept(false) // throws zsLib::Exceptions::InvalidArgument
      {
        importFromString(zsLib::ISettings::getString(key).c_str());
      }

      //-----------------------------------------------------------------------
      void importFromString(const char *previousExportString) noexcept(false) // throws zsLib::Exceptions::InvalidArgument
      {
        typedef zsLib::Numeric<UseRangeType> UseNumeric;
        typedef typename UseNumeric::ValueOutOfRange ValueOutOfRange;

        reset();
        auto doc = Document::createFromAutoDetect(previousExportString);
        if (!doc) return;

        auto rootEl = doc->getFirstChildElement();
        if (!rootEl) return;

        auto allowsEl = rootEl->findFirstChildElement("allows");
        if (allowsEl) {
          auto allowEl = allowsEl->findFirstChildElement("allow");
          while (allowEl) {
            auto startEl = allowEl->findFirstChildElement("start");
            auto endEl = allowEl->findFirstChildElement("end");

            if ((startEl) &&
                (endEl)) {
              auto startStr = startEl->getText();
              auto endStr = endEl->getText();

              try {
                UseRangeType start = UseNumeric(startStr);
                UseRangeType end = UseNumeric(endStr);
                allow(start, end);
              } catch (const ValueOutOfRange &) {
                throwRangeSelectionInvalidArgumentStartStopStr(startStr, endStr);
              }
            }
            allowEl = allowEl->findNextSiblingElement("allow");
          }
        }


        auto deniesEl = rootEl->findFirstChildElement("denies");
        if (deniesEl) {
          auto denyEl = deniesEl->findFirstChildElement("deny");
          while (denyEl) {
            auto startEl = denyEl->findFirstChildElement("start");
            auto endEl = denyEl->findFirstChildElement("end");

            if ((startEl) &&
              (endEl)) {
              auto startStr = startEl->getText();
              auto endStr = endEl->getText();

              try {
                UseRangeType start = UseNumeric(startStr);
                UseRangeType end = UseNumeric(endStr);
                deny(start, end);
              } catch (const ValueOutOfRange &) {
                throwRangeSelectionInvalidArgumentStartStopStr(startStr, endStr);
              }
            }
            denyEl = denyEl->findNextSiblingElement("deny");
          }
        }
      }

      //-----------------------------------------------------------------------
      void exportToSetting(const char *key) const noexcept
      {
        zsLib::ISettings::setString(key, exportToString().c_str());
      }

      //-----------------------------------------------------------------------
      String exportToString() const noexcept
      {
        auto doc = Document::create();
        auto rootEl = Element::create("range");

        if (allows_.size() > 0) {
          auto allowsEl = Element::create("allows");
          for (auto iter = allows_.begin(); iter != allows_.end(); ++iter) {
            auto &range = (*iter);
            String startStr = string(range.first);
            String endStr = string(range.second);

            auto allowEl = Element::create("allow");
            auto startEl = Element::create("start");
            auto endEl = Element::create("end");

            { auto textEl = Text::create(); textEl->setValue(startStr); textEl->setOutputFormat(Text::Format_JSONNumberEncoded); startEl->adoptAsLastChild(textEl); }
            { auto textEl = Text::create(); textEl->setValue(endStr); textEl->setOutputFormat(Text::Format_JSONNumberEncoded); endEl->adoptAsLastChild(textEl); }

            allowEl->adoptAsLastChild(startEl);
            allowEl->adoptAsLastChild(endEl);
            allowsEl->adoptAsLastChild(allowEl);
          }
          rootEl->adoptAsLastChild(allowsEl);
        }
        if (denies_.size() > 0) {
          auto deniesEl = Element::create("denies");
          for (auto iter = denies_.begin(); iter != denies_.end(); ++iter) {
            auto &range = (*iter);
            String startStr = string(range.first);
            String endStr = string(range.second);

            auto denyEl = Element::create("deny");
            auto startEl = Element::create("start");
            auto endEl = Element::create("end");

            { auto textEl = Text::create(); textEl->setValue(startStr); textEl->setOutputFormat(Text::Format_JSONNumberEncoded); startEl->adoptAsLastChild(textEl); }
            { auto textEl = Text::create(); textEl->setValue(endStr); textEl->setOutputFormat(Text::Format_JSONNumberEncoded); endEl->adoptAsLastChild(textEl); }

            denyEl->adoptAsLastChild(startEl);
            denyEl->adoptAsLastChild(endEl);
            deniesEl->adoptAsLastChild(denyEl);
          }
          rootEl->adoptAsLastChild(deniesEl);
        }

        doc->adoptAsLastChild(rootEl);
        auto resultBuffer = doc->writeAsJSON();

        String result(resultBuffer.get());
        return result;
      }

      //-----------------------------------------------------------------------
      void reset() noexcept
      {
        dirty_ = true;
        allows_.clear();
        denies_.clear();
      }

      //-----------------------------------------------------------------------
      void allow(
                 UseRangeType from,
                 UseRangeType to
                 ) noexcept
      {
        dirty_ = true;
        if (from > to) return;
        allows_.push_back(RangeTypePair(from, to));
      }

      //-----------------------------------------------------------------------
      void deny(
                UseRangeType from,
                UseRangeType to
                ) noexcept
      {
        dirty_ = true;
        if (from > to) return;
        denies_.push_back(RangeTypePair(from, to));
      }

      //-----------------------------------------------------------------------
      void removeAllow(
                       UseRangeType from,
                       UseRangeType to
                       ) noexcept
      {
        if (from > to) return;
        for (auto iter_doNotUse = allows_.begin(); iter_doNotUse != allows_.end(); ) {
          auto current = iter_doNotUse;
          ++iter_doNotUse;

          auto &range = (*current);
          if (range.first != from) continue;
          if (range.second != to) continue;
          dirty_ = true;
          allows_.erase(current);
        }
      }

      //-----------------------------------------------------------------------
      void removeDeny(
                      UseRangeType from,
                      UseRangeType to
                      ) noexcept
      {
        if (from > to) return;
        for (auto iter_doNotUse = denies_.begin(); iter_doNotUse != denies_.end(); ) {
          auto current = iter_doNotUse;
          ++iter_doNotUse;

          auto &range = (*current);
          if (range.first != from) continue;
          if (range.second != to) continue;
          dirty_ = true;
          allows_.erase(current);
        }
      }

      //-----------------------------------------------------------------------
      UseRangeType getRandomPosition(UseLargeUnsignedType randomInputValue) noexcept(false) // throws ::zsLib::Exceptions::BadState
      {
        calculate();
        throwRangeSelectionBadStateIf(count_ < 1, "count_ < 1");

        UseLargeUnsignedType choice = randomInputValue % count_;

        for (auto iter = allowed_.begin(); iter != allowed_.end(); ++iter) {
          auto &range = (*iter).second;
          UseLargeUnsignedType rangeCount = 0;
          if (std::numeric_limits<UseRangeType>::is_signed) {
            LargeSignedType value1 = SafeInt<LargeSignedType>(range.first);
            LargeSignedType value2 = SafeInt<LargeSignedType>(range.first);
            rangeCount  = (UseLargeUnsignedType)(value2 - value1);
          } else {
            rangeCount = SafeInt<UseLargeUnsignedType>(range.second - range.first);
          }
          if (rangeCount < std::numeric_limits<UseLargeUnsignedType>::max()) ++rangeCount;

          if (choice >= rangeCount) {
            choice -= rangeCount;
            continue;
          }
          return range.first + SafeInt<UseRangeType>(choice);
        }
        throwRangeSelectionBadState("selection not found within calculated range"); // not legal
        return UseRangeType {};
      }

      //-----------------------------------------------------------------------
      bool isAllowed(UseRangeType value) noexcept
      {
        calculate();
        for (auto iter = allowed_.begin(); iter != allowed_.end(); ++iter) {
          auto &range = (*iter).second;

          if ((value >= range.first) &&
              (value <= range.second)) return true;
        }

        return false;
      }

    protected:
      //-----------------------------------------------------------------------
      void calculate() noexcept
      {
        if (!dirty_) return;
        allowed_.clear();
        calculateAllows();
        calculateDenies();
        calculateCount();
        dirty_ = false;
      }

      //-----------------------------------------------------------------------
      void calculateAllows() noexcept
      {
        if (allows_.size() < 1) {
          // legal range is entire scope of values if allowed is not specified
          allowed_[std::numeric_limits<UseRangeType>::lowest()] = RangeTypePair(std::numeric_limits<UseRangeType>::lowest(), std::numeric_limits<UseRangeType>::max());
          return;
        }

        for (auto iterOuter = allows_.begin(); iterOuter != allows_.end(); ++iterOuter) {
          auto &checkRange = (*iterOuter);

          // scope: calculate allows
          {
            // check if the range exists inside an already defined range
            for (auto iterInner = allowed_.begin(); iterInner != allowed_.end(); ++iterInner) {
              auto &existingRange = (*iterInner).second;

              if ((checkRange.first >= existingRange.first) &&
                  (checkRange.first <= existingRange.second) &&
                  (checkRange.second >= existingRange.first) &&
                  (checkRange.second <= existingRange.second)) {
                goto skip_check;
              }
            }

            // remove all ranges that will be entirely within the new range
            for (auto iterInner_doNotUse = allowed_.begin(); iterInner_doNotUse != allowed_.end(); ) {
              auto current = iterInner_doNotUse;
              ++iterInner_doNotUse;

              auto &existingRange = (*current).second;

              if ((existingRange.first >= checkRange.first) &&
                  (existingRange.first <= checkRange.second) &&
                  (existingRange.second >= checkRange.first) &&
                  (existingRange.second <= checkRange.second)) {
                allowed_.erase(current);
              }
            }

            // if there's no overlap at all just insert the new range
            bool overlapFound = false;
            for (auto iterInner = allowed_.begin(); iterInner != allowed_.end(); ++iterInner) {
              auto &existingRange = (*iterInner).second;

              if ((checkRange.first >= existingRange.first) &&
                  (checkRange.first <= existingRange.second)) {
                overlapFound = true;
                break;
              }
              if ((checkRange.second >= existingRange.first) &&
                  (checkRange.second <= existingRange.second)) {
                overlapFound = true;
                break;
              }
            }
            if (!overlapFound) {
              allowed_[checkRange.first] = checkRange;
              goto skip_check;
            }

            // overlap found thus this is a bit more complex a scenario
            typename RangeMap::iterator foundStart = allowed_.end();
            typename RangeMap::iterator foundEnd = allowed_.end();

            for (auto iterInner = allowed_.begin(); iterInner != allowed_.end(); ++iterInner) {
              auto &existingRange = (*iterInner).second;

              if ((checkRange.first >= existingRange.first) &&
                  (checkRange.first <= existingRange.second)) {
                foundStart = iterInner;
              }
              if ((checkRange.second >= existingRange.first) &&
                  (checkRange.second <= existingRange.second)) {
                foundEnd = iterInner;
              }
            }

            if (foundStart != allowed_.end()) {
              RangeTypePair newRange {};
              newRange.first = (*foundStart).second.first;
              newRange.second = (*foundStart).second.second;
              if (newRange.second < checkRange.second) newRange.second = checkRange.second;

              allowed_.erase(foundStart);
              if (foundEnd != allowed_.end()) {
                if (newRange.second <= (*foundEnd).second.second) newRange.second = (*foundEnd).second.second;
                allowed_.erase(foundEnd);
              }
              allowed_[newRange.first] = newRange;
              goto skip_check;
            }
            if (foundEnd != allowed_.end()) {
              RangeTypePair newRange {};
              newRange.first = checkRange.first;
              newRange.second = (*foundEnd).second.second;
              if (newRange.second < checkRange.second) newRange.second = checkRange.second;
              allowed_.erase(foundEnd);
              allowed_[newRange.first] = newRange;
              goto skip_check;
            }
            goto skip_check;
          }

        skip_check: {}
        }
      }

      //-----------------------------------------------------------------------
      void calculateDenies() noexcept
      {
        if (denies_.size() < 1) return;

        RangeTypeList tempDenies = denies_;

        // remove any denied ranges that do not overlap any existing allowed ranges
        for (auto iterOuter_doNotUse = tempDenies.begin(); iterOuter_doNotUse != tempDenies.end(); ) {
          auto currentOuter = iterOuter_doNotUse;
          ++iterOuter_doNotUse;

          auto &denyRange = (*currentOuter);

          // check if the denial range exists within any of the allowed range
          bool foundInAllowed = false;

          for (auto iterInner = allowed_.begin(); iterInner != allowed_.end(); ++iterInner) {
            auto &allowRange = (*iterInner).second;

            if ((denyRange.first < allowRange.first) &&
                (denyRange.second < allowRange.first)) continue;
            if ((denyRange.first > allowRange.second) &&
                (denyRange.second > allowRange.second)) continue;
            foundInAllowed = true;
            break;
          }

          if (foundInAllowed) continue;
          tempDenies.erase(currentOuter);
        }

        if (tempDenies.size() < 1) return;

        // remove any allowed ranges that are entirely within a denied range
        for (auto iterOuter = tempDenies.begin(); iterOuter != tempDenies.end(); ++iterOuter) {
          auto &denyRange = (*iterOuter);

          for (auto iterInner_doNotUse = allowed_.begin(); iterInner_doNotUse != allowed_.end(); ) {
            auto currentInner = iterInner_doNotUse;
            ++iterInner_doNotUse;

            auto &allowRange = (*currentInner).second;

            if ((allowRange.first >= denyRange.first) &&
                (allowRange.first <= denyRange.second) &&
                (allowRange.second >= denyRange.first) &&
                (allowRange.second <= denyRange.second)) {
              allowed_.erase(currentInner);
              continue;
            }
          }
        }

        if (tempDenies.size() < 1) return;

        // split any allowed range where a denial range is in the middle of the allowed range
        for (auto iterOuter_doNotUse = tempDenies.begin(); iterOuter_doNotUse != tempDenies.end(); ) {
          auto currentOuter = iterOuter_doNotUse;
          auto &denyRange = (*currentOuter);
          ++iterOuter_doNotUse;

          // scope: process denial splits
          {
            for (auto iterInner_doNotUse = allowed_.begin(); iterInner_doNotUse != allowed_.end(); ) {
              auto currentInner = iterInner_doNotUse;
              ++iterInner_doNotUse;

              auto &allowRange = (*currentInner).second;

              if (!((denyRange.first >= allowRange.first) &&
                    (denyRange.first <= allowRange.second) &&
                    (denyRange.second >= allowRange.first) &&
                    (denyRange.second <= allowRange.second))) {
                // the deny range is not within the existing range
                continue;
              }

              // special cases - denial start/stop is exactly at allow start/stop
              if (denyRange.first == allowRange.first) {
                if (denyRange.second == allowRange.second) {
                  tempDenies.erase(currentOuter);
                  allowed_.erase(currentInner);
                  goto process_next_denial_split;
                }
                // start of allow range needs to move after denial range
                RangeTypePair newRange {};
                newRange.first = denyRange.second + 1;
                newRange.second = allowRange.second;
                tempDenies.erase(currentOuter);
                allowed_.erase(currentInner);
                if (newRange.first <= newRange.second) {
                  allowed_[newRange.first] = newRange;
                }
                goto process_next_denial_split;
              }

              if (denyRange.second == allowRange.second) {
                RangeTypePair newRange {};
                newRange.first = allowRange.first;
                newRange.second = denyRange.first-1;
                tempDenies.erase(currentOuter);
                allowed_.erase(currentInner);
                if (newRange.first <= newRange.second) {
                  allowed_[newRange.first] = newRange;
                }
                goto process_next_denial_split;
              }

              RangeTypePair newRange1 {};
              newRange1.first = allowRange.first;
              newRange1.second = denyRange.first - 1;
              RangeTypePair newRange2 {};
              newRange2.first = denyRange.second + 1;
              newRange2.second = allowRange.second;
              tempDenies.erase(currentOuter);
              allowed_.erase(currentInner);
              if (newRange1.first <= newRange1.second) {
                allowed_[newRange1.first] = newRange1;
              }
              if (newRange2.first <= newRange2.second) {
                allowed_[newRange2.first] = newRange2;
              }
              goto process_next_denial_split;
            }
          }
        process_next_denial_split: {}
        }

        if (tempDenies.size() < 1) return;

        // shrink any allowed start range that falls inside a denial
        for (auto iterOuter_doNotUse = tempDenies.begin(); iterOuter_doNotUse != tempDenies.end(); ) {
          auto currentOuter = iterOuter_doNotUse;
          auto &denyRange = (*currentOuter);
          ++iterOuter_doNotUse;

          for (auto iterInner_doNotUse = allowed_.begin(); iterInner_doNotUse != allowed_.end(); ) {
            auto currentInner = iterInner_doNotUse;
            ++iterInner_doNotUse;

            auto &allowRange = (*currentInner).second;

            // make sure the allowed start range lies within the denial range
            if (!((allowRange.first >= denyRange.first) &&
                  (allowRange.first <= denyRange.second))) {
              continue;
            }

            RangeTypePair newRange {};
            newRange.first = denyRange.second + 1;
            newRange.second = allowRange.second;
            tempDenies.erase(currentOuter);
            allowed_.erase(currentInner);
            if (newRange.first <= newRange.second) {
              allowed_[newRange.first] = newRange;
            }
          }
        }

        if (tempDenies.size() < 1) return;

        // shrink any allowed end range that falls inside a denial
        for (auto iterOuter_doNotUse = tempDenies.begin(); iterOuter_doNotUse != tempDenies.end(); ) {
          auto currentOuter = iterOuter_doNotUse;
          auto &denyRange = (*currentOuter);
          ++iterOuter_doNotUse;

          for (auto iterInner_doNotUse = allowed_.begin(); iterInner_doNotUse != allowed_.end(); ) {
            auto currentInner = iterInner_doNotUse;
            ++iterInner_doNotUse;

            auto &allowRange = (*currentInner).second;

            // make sure the allowed start range lies within the denial range
            if (!((allowRange.second >= denyRange.first) &&
                  (allowRange.second <= denyRange.second))) {
              continue;
            }

            RangeTypePair newRange {};
            newRange.first = allowRange.first;
            newRange.second = denyRange.first - 1;
            tempDenies.erase(currentOuter);
            allowed_.erase(currentInner);
            if (newRange.first <= newRange.second) {
              allowed_[newRange.first] = newRange;
            }
          }
        }
      }

      //-----------------------------------------------------------------------
      void calculateCount() noexcept
      {
        count_ = 0;

        for (auto iter = allowed_.begin(); iter != allowed_.end(); ++iter) {
          auto &range = (*iter).second;

          count_ += SafeInt<UseLargeUnsignedType>(range.second - range.first);
          if (count_ < std::numeric_limits<UseLargeUnsignedType>::max()) { ++count_; }
        }
      }

    protected:
      //-----------------------------------------------------------------------
      //-----------------------------------------------------------------------
      //-----------------------------------------------------------------------
      //-----------------------------------------------------------------------
      //
      // RangeSelection => (data)
      //

      bool dirty_ {true};

      RangeTypeList allows_;
      RangeTypeList denies_;

      RangeMap allowed_;
      UseLargeUnsignedType count_ {};
    };

  } // namespace internal
} // namespace zsLib

#ifdef ZSLIB_RANGE_UNDEFED_MAX
// put back the max() definition
#define max(a,b) (((a) > (b)) ? (a) : (b))
#undef ZSLIB_RANGE_UNDEFED_MAX
#endif // ZSLIB_RANGE_UNDEFED_MAX
