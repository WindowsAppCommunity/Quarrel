/*

 Copyright (c) 2014, Robin Raymond
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
#include <zsLib/Stringize.h>
#include <zsLib/eventing/EventTypes.h>

#include <list>
#include <map>
#include <set>

#define ZSLIB_LOG_PROVIDER_ATOM_MAXIMUM (256)
#define ZSLIB_LOG_PROVIDER_KEYWORDS_ALL (0xFFFFFFFFFFFFFFFFULL)
#define ZSLIB_LOG_PROVIDER_KEYWORDS_NO_KEYWORD_DEFINED (0x8000000000000000ULL)

#define ZSLIB_INTERNAL_LOG_EVENT_WRITER_INIT_VALUE (0xABCDEF98)

namespace zsLib
{
  class String;
  class Exception;

  class Subsystem;

  namespace internal
  {
    class Log
    {
    protected:
      struct make_private {};
      struct EventingWriter;
      typedef int LevelBaseType;

      typedef std::list<ILogOutputDelegatePtr> OutputListenerList;
      ZS_DECLARE_PTR(OutputListenerList);
      typedef std::list<ILogEventingDelegatePtr> EventingListenerList;
      ZS_DECLARE_PTR(EventingListenerList);
      typedef std::list<ILogEventingProviderDelegatePtr> EventingProviderListenerList;
      ZS_DECLARE_PTR(EventingProviderListenerList);

      typedef std::list<Subsystem *> SubsystemList;

      typedef String SubsystemName;
      typedef std::map<SubsystemName, LevelBaseType> SubsystemLevelMap;

      typedef uint64_t InternalKeywordBitmaskType;
      typedef size_t InternalAtomIndex;
      typedef uintptr_t InternalAtomData;
      
      typedef String AtomNamespace;
      typedef std::map<AtomNamespace, InternalAtomIndex> AtomIndexMap;

      typedef std::set<EventingWriter *> EventWriterSet;
      typedef std::map<UUID, EventingWriter *> EventWriterMap;

      typedef PUID ObjectID;
      typedef std::map<ObjectID, InternalKeywordBitmaskType> ObjectIDBitmaskMap;

      struct EventingWriter
      {
        EventingWriter() noexcept;
        EventingWriter(const EventingWriter &op2) = delete;

        ~EventingWriter() noexcept;

        volatile InternalKeywordBitmaskType mKeywordsBitmask {0};
        volatile uint32_t mInitValue {ZSLIB_INTERNAL_LOG_EVENT_WRITER_INIT_VALUE};
        UUID mProviderID {};
        String mProviderName;
        String mUniqueProviderHash;
        String mJMAN;
        zsLib::Log *mLog{};
        InternalAtomData mAtomInfo[ZSLIB_LOG_PROVIDER_ATOM_MAXIMUM]{};
        ObjectIDBitmaskMap mEnabledObjects;

        EventingWriter &operator=(const EventingWriter &op2) = delete;
      };

    public:
      Log(const make_private &) noexcept;
      ~Log() noexcept;

      static void initSingleton() noexcept;

      static String paramize(const char *name) noexcept;

      constexpr static bool returnFalse() noexcept { return false; }
      constexpr static bool returnTrue() noexcept { return true; }

    protected:
      RecursiveLock mLock;

      OutputListenerListPtr mOutputListeners;
      EventingListenerListPtr mEventingListeners;
      EventingProviderListenerListPtr mEventingProviderListeners;

      SubsystemList mSubsystems;

      SubsystemLevelMap mDefaultOutputSubsystemLevels;
      SubsystemLevelMap mDefaultEventingSubsystemLevels;
      SubsystemLevelMap mEventingSubsystemLevels;

      EventWriterMap mEventWriters;
      EventWriterSet mCleanUpWriters;

      AtomIndexMap mConsumedEventingAtoms;
    };
  }
}


#define ZS_INTERNAL_DECLARE_FORWARD_SUBSYSTEM(xSubsystem) \
  ::zsLib::Subsystem &get##xSubsystem##Subsystem();       \

#define ZS_INTERNAL_DECLARE_SUBSYSTEM(xSubsystem)         \
  ZS_MAYBE_USED() ::zsLib::Subsystem &get##xSubsystem##Subsystem();       \
  ZS_MAYBE_USED() static ::zsLib::Subsystem &getCurrentSubsystem() {      \
    return get##xSubsystem##Subsystem();                  \
  }

#define ZS_INTERNAL_IMPLEMENT_SUBSYSTEM(xSubsystem)       \
  class xSubsystem##Subsystem : public ::zsLib::Subsystem \
  {                                                       \
  public:                                                 \
    xSubsystem##Subsystem(::zsLib::CSTR inName = #xSubsystem) : ::zsLib::Subsystem(inName) {notifyNewSubsystem();} \
  };                                                      \
  ::zsLib::Subsystem &get##xSubsystem##Subsystem() {      \
    static xSubsystem##Subsystem subsystem;               \
    return subsystem;                                     \
    }

#define ZS_INTERNAL_GET_SUBSYSTEM()                                       (getCurrentSubsystem())
#define ZS_INTERNAL_GET_OTHER_SUBSYSTEM(xNamespace, xSubsystem)           (xNamespace::get##xSubsystem##Subsystem())

#define ZS_INTERNAL_GET_CURRENT_TIMESTAMP_MS()                            (::zsLib::Log::getCurrentTimestampMS())
#define ZS_INTERNAL_GET_CURRENT_THREAD_ID()                               (::zsLib::Log::getCurrentThreadID())

#define ZS_INTERNAL_LOG_PARAMS(xMsg)                                      (::zsLib::Log::Params(xMsg))
#define ZS_INTERNAL_PARAM(xName, xValue)                                  (::zsLib::Log::Param(xName, xValue))
#define ZS_INTERNAL_PARAMIZE(xValueName)                                  (::zsLib::Log::Param(::zsLib::internal::Log::paramize(#xValueName), xValueName))

#define ZS_INTERNAL_FUNCTION_FILE_LINE                                    __FUNCTION__, __FILE__, __LINE__

#define ZS_INTERNAL_GET_LOG_LEVEL()                                       ((ZS_GET_SUBSYSTEM()).getOutputLevel())
#define ZS_INTERNAL_GET_SUBSYSTEM_LOG_LEVEL(xSubsystem)                   ((xSubsystem).getOutputLevel())

#define ZS_INTERNAL_GET_CURRENT_SUBSYSTEM_NAME()                          ((ZS_GET_SUBSYSTEM()).getName())
#define ZS_INTERNAL_GET_SUBSYSTEM_NAME(xSubsystem)                        ((xSubsystem).getName())

#ifdef ZSLIB_LOGGING_NOOP

#define ZS_INTERNAL_IS_LOGGING(xLevel)                                    (::zsLib::internal::Log::returnFalse())
#define ZS_INTERNAL_IS_LOGGING_VALUE(xLevelValue)                         (::zsLib::internal::Log::returnFalse())
#define ZS_INTERNAL_IS_SUBSYSTEM_LOGGING(xSubsystem, xLevel)              (::zsLib::internal::Log::returnFalse())

#define ZS_INTERNAL_LOG_SUBSYSTEM_BASIC(xSubsystem, xMsg)                 ;
#define ZS_INTERNAL_LOG_SUBSYSTEM_DETAIL(xSubsystem, xMsg)                ;
#define ZS_INTERNAL_LOG_SUBSYSTEM_DEBUG(xSubsystem, xMsg)                 ;
#define ZS_INTERNAL_LOG_SUBSYSTEM_TRACE(xSubsystem, xMsg)                 ;
#define ZS_INTERNAL_LOG_SUBSYSTEM_INSANE(xSubsystem, xMsg)                ;

#define ZS_INTERNAL_LOG_FORCED(xSeverity, xLevel, xMsg)                   ;

#define ZS_INTERNAL_LOG_BASIC(xMsg)                                       ;
#define ZS_INTERNAL_LOG_DETAIL(xMsg)                                      ;
#define ZS_INTERNAL_LOG_DEBUG(xMsg)                                       ;
#define ZS_INTERNAL_LOG_TRACE(xMsg)                                       ;
#define ZS_INTERNAL_LOG_INSANE(xMsg)                                      ;

#define ZS_INTERNAL_LOG_WITH_SEVERITY(xSeverityValue, xLevelValue, xMsg)  ;

#define ZS_INTERNAL_LOG_BASIC_WITH_SEVERITY(xSeverityValue, xMsg)         ;
#define ZS_INTERNAL_LOG_DETAIL_WITH_SEVERITY(xSeverityValue, xMsg)        ;
#define ZS_INTERNAL_LOG_DEBUG_WITH_SEVERITY(xSeverityValue, xMsg)         ;
#define ZS_INTERNAL_LOG_TRACE_WITH_SEVERITY(xSeverityValue, xMsg)         ;
#define ZS_INTERNAL_LOG_INSANE_WITH_SEVERITY(xSeverityValue, xMsg)        ;

#define ZS_INTERNAL_LOG_SUBSYSTEM(xSubsystem, xLevel, xMsg)               ;
#define ZS_INTERNAL_LOG_SUBSYSTEM_WARNING(xSubsystem, xLevel, xMsg)       ;
#define ZS_INTERNAL_LOG_SUBSYSTEM_ERROR(xSubsystem, xLevel, xMsg)         ;
#define ZS_INTERNAL_LOG_SUBSYSTEM_FATAL(xSubsystem, xLevel, xMsg)         ;

#define ZS_INTERNAL_LOG(xLevel, xMsg)                                     ;
#define ZS_INTERNAL_LOG_WARNING(xLevel, xMsg)                             ;
#define ZS_INTERNAL_LOG_ERROR(xLevel, xMsg)                               ;
#define ZS_INTERNAL_LOG_FATAL(xLevel, xMsg)                               ;

#define ZS_INTERNAL_LOG_IF(xCond, xLevel, xMsg)                           ;
#define ZS_INTERNAL_LOG_WARNING_IF(xCond, xLevel, xMsg)                   ;
#define ZS_INTERNAL_LOG_ERROR_IF(xCond, xLevel, xMsg)                     ;
#define ZS_INTERNAL_LOG_FATAL_IF(xCond, xLevel, xMsg)                     ;

#else /* ZSLIB_LOGGING_NOOP */

#define ZS_INTERNAL_IS_LOGGING(xLevel)                                    (((ZS_GET_SUBSYSTEM()).getOutputLevel()) >= ::zsLib::Log::xLevel)
#define ZS_INTERNAL_IS_LOGGING_VALUE(xLevelValue)                         (((ZS_GET_SUBSYSTEM()).getOutputLevel()) >= (xLevelValue))
#define ZS_INTERNAL_IS_SUBSYSTEM_LOGGING(xSubsystem, xLevel)              (((xSubsystem).getOutputLevel()) >= ::zsLib::Log::xLevel)

#define ZS_INTERNAL_LOG_SUBSYSTEM_BASIC(xSubsystem, xMsg)                 if (ZS_INTERNAL_IS_LOGGING(Basic))  {::zsLib::Log::log((xSubsystem), ::zsLib::Log::Informational, ::zsLib::Log::Basic, ::zsLib::Log::Params(xMsg), ZS_INTERNAL_FUNCTION_FILE_LINE);}
#define ZS_INTERNAL_LOG_SUBSYSTEM_DETAIL(xSubsystem, xMsg)                if (ZS_INTERNAL_IS_LOGGING(Detail)) {::zsLib::Log::log((xSubsystem), ::zsLib::Log::Informational, ::zsLib::Log::Detail, ::zsLib::Log::Params(xMsg), ZS_INTERNAL_FUNCTION_FILE_LINE);}
#define ZS_INTERNAL_LOG_SUBSYSTEM_DEBUG(xSubsystem, xMsg)                 if (ZS_INTERNAL_IS_LOGGING(Debug))  {::zsLib::Log::log((xSubsystem), ::zsLib::Log::Informational, ::zsLib::Log::Debug, ::zsLib::Log::Params(xMsg), ZS_INTERNAL_FUNCTION_FILE_LINE);}
#define ZS_INTERNAL_LOG_SUBSYSTEM_TRACE(xSubsystem, xMsg)                 if (ZS_INTERNAL_IS_LOGGING(Trace))  {::zsLib::Log::log((xSubsystem), ::zsLib::Log::Informational, ::zsLib::Log::Trace, ::zsLib::Log::Params(xMsg), ZS_INTERNAL_FUNCTION_FILE_LINE);}
#define ZS_INTERNAL_LOG_SUBSYSTEM_INSANE(xSubsystem, xMsg)                if (ZS_INTERNAL_IS_LOGGING(Insane)) {::zsLib::Log::log((xSubsystem), ::zsLib::Log::Informational, ::zsLib::Log::Insane, ::zsLib::Log::Params(xMsg), ZS_INTERNAL_FUNCTION_FILE_LINE);}

#define ZS_INTERNAL_LOG_FORCED(xSeverity, xLevel, xMsg)                   {::zsLib::Log::log(ZS_GET_SUBSYSTEM(), ::zsLib::Log::xSeverity, ::zsLib::Log::xLevel, ::zsLib::Log::Params(xMsg), ZS_INTERNAL_FUNCTION_FILE_LINE);}

#define ZS_INTERNAL_LOG_BASIC(xMsg)                                       ZS_INTERNAL_LOG_SUBSYSTEM_BASIC(ZS_GET_SUBSYSTEM(), xMsg)
#define ZS_INTERNAL_LOG_DETAIL(xMsg)                                      ZS_INTERNAL_LOG_SUBSYSTEM_DETAIL(ZS_GET_SUBSYSTEM(), xMsg)
#define ZS_INTERNAL_LOG_DEBUG(xMsg)                                       ZS_INTERNAL_LOG_SUBSYSTEM_DEBUG(ZS_GET_SUBSYSTEM(), xMsg)
#define ZS_INTERNAL_LOG_TRACE(xMsg)                                       ZS_INTERNAL_LOG_SUBSYSTEM_TRACE(ZS_GET_SUBSYSTEM(), xMsg)
#define ZS_INTERNAL_LOG_INSANE(xMsg)                                      ZS_INTERNAL_LOG_SUBSYSTEM_INSANE(ZS_GET_SUBSYSTEM(), xMsg)

#define ZS_INTERNAL_LOG_WITH_SEVERITY(xSeverityValue, xLevelValue, xMsg)  if (ZS_INTERNAL_IS_LOGGING_VALUE(xLevelValue)) {::zsLib::Log::log(ZS_GET_SUBSYSTEM(), (xSeverityValue), (xLevelValue), ::zsLib::Log::Params(xMsg), ZS_INTERNAL_FUNCTION_FILE_LINE);}

#define ZS_INTERNAL_LOG_BASIC_WITH_SEVERITY(xSeverityValue, xMsg)         ZS_INTERNAL_LOG_WITH_SEVERITY((xSeverityValue), ::zsLib::Log::Basic, xMsg)
#define ZS_INTERNAL_LOG_DETAIL_WITH_SEVERITY(xSeverityValue, xMsg)        ZS_INTERNAL_LOG_WITH_SEVERITY((xSeverityValue), ::zsLib::Log::Detail, xMsg)
#define ZS_INTERNAL_LOG_DEBUG_WITH_SEVERITY(xSeverityValue, xMsg)         ZS_INTERNAL_LOG_WITH_SEVERITY((xSeverityValue), ::zsLib::Log::Debug, xMsg)
#define ZS_INTERNAL_LOG_TRACE_WITH_SEVERITY(xSeverityValue, xMsg)         ZS_INTERNAL_LOG_WITH_SEVERITY((xSeverityValue), ::zsLib::Log::Trace, xMsg)
#define ZS_INTERNAL_LOG_INSANE_WITH_SEVERITY(xSeverityValue, xMsg)        ZS_INTERNAL_LOG_WITH_SEVERITY((xSeverityValue), ::zsLib::Log::Insane, xMsg)

#define ZS_INTERNAL_LOG_SUBSYSTEM(xSubsystem, xLevel, xMsg)               if (ZS_INTERNAL_IS_LOGGING(xLevel)) {::zsLib::Log::log((xSubsystem), ::zsLib::Log::Informational, ::zsLib::Log::xLevel, ::zsLib::Log::Params(xMsg), ZS_INTERNAL_FUNCTION_FILE_LINE);}
#define ZS_INTERNAL_LOG_SUBSYSTEM_WARNING(xSubsystem, xLevel, xMsg)       if (ZS_INTERNAL_IS_LOGGING(xLevel)) {::zsLib::Log::log((xSubsystem), ::zsLib::Log::Warning, ::zsLib::Log::xLevel, ::zsLib::Log::Params(xMsg), ZS_INTERNAL_FUNCTION_FILE_LINE);}
#define ZS_INTERNAL_LOG_SUBSYSTEM_ERROR(xSubsystem, xLevel, xMsg)         if (ZS_INTERNAL_IS_LOGGING(xLevel)) {::zsLib::Log::log((xSubsystem), ::zsLib::Log::Error, ::zsLib::Log::xLevel, ::zsLib::Log::Params(xMsg), ZS_INTERNAL_FUNCTION_FILE_LINE);}
#define ZS_INTERNAL_LOG_SUBSYSTEM_FATAL(xSubsystem, xLevel, xMsg)         if (ZS_INTERNAL_IS_LOGGING(xLevel)) {::zsLib::Log::log((xSubsystem), ::zsLib::Log::Fatal, ::zsLib::Log::xLevel, ::zsLib::Log::Params(xMsg), ZS_INTERNAL_FUNCTION_FILE_LINE);}

#define ZS_INTERNAL_LOG(xLevel, xMsg)                                     ZS_INTERNAL_LOG_SUBSYSTEM(ZS_GET_SUBSYSTEM(), xLevel, xMsg)
#define ZS_INTERNAL_LOG_WARNING(xLevel, xMsg)                             ZS_INTERNAL_LOG_SUBSYSTEM_WARNING(ZS_GET_SUBSYSTEM(), xLevel, xMsg)
#define ZS_INTERNAL_LOG_ERROR(xLevel, xMsg)                               ZS_INTERNAL_LOG_SUBSYSTEM_ERROR(ZS_GET_SUBSYSTEM(), xLevel, xMsg)
#define ZS_INTERNAL_LOG_FATAL(xLevel, xMsg)                               ZS_INTERNAL_LOG_SUBSYSTEM_FATAL(ZS_GET_SUBSYSTEM(), xLevel, xMsg)

#define ZS_INTERNAL_LOG_IF(xCond, xLevel, xMsg)                           if ((xCond) && (ZS_INTERNAL_IS_LOGGING(xLevel))) {ZS_INTERNAL_LOG(xLevel, xMsg)}
#define ZS_INTERNAL_LOG_WARNING_IF(xCond, xLevel, xMsg)                   if ((xCond) && (ZS_INTERNAL_IS_LOGGING(xLevel))) {ZS_INTERNAL_LOG_WARNING(xLevel, xMsg)}
#define ZS_INTERNAL_LOG_ERROR_IF(xCond, xLevel, xMsg)                     if ((xCond) && (ZS_INTERNAL_IS_LOGGING(xLevel))) {ZS_INTERNAL_LOG_ERROR(xLevel, xMsg)}
#define ZS_INTERNAL_LOG_FATAL_IF(xCond, xLevel, xMsg)                     if ((xCond) && (ZS_INTERNAL_IS_LOGGING(xLevel))) {ZS_INTERNAL_LOG_FATAL(xLevel, xMsg)}

#endif /* ZSLIB_LOGGING_NOOP */
