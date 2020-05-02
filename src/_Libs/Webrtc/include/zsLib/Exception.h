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

#include <zsLib/internal/zsLib_Exception.h>

namespace zsLib
{
  class Exception : public std::exception
  {
  public:
    Exception(
              const Subsystem &subsystem,
              CSTR message,
              CSTR function,
              CSTR filePath,
              ULONG lineNumber,
              const char *expression = NULL
              ) noexcept;
    Exception(
              const Subsystem &subsystem,
              const String &message,
              CSTR function,
              CSTR filePath,
              ULONG lineNumber,
              const char *expression = NULL
              ) noexcept;
    Exception(
              const Subsystem &subsystem,
              const Log::Params &params,
              CSTR function,
              CSTR filePath,
              ULONG lineNumber,
              const char *expression = NULL
              ) noexcept;
    Exception(const Exception &op2) noexcept;

    ~Exception() noexcept {}

    virtual const char *what() const noexcept;

    const Subsystem &subsystem() const noexcept {return mSubsystem;}
    const String &message() const noexcept {return mMessage;}
    CSTR function() const noexcept {return mFunction;}
    CSTR filePath() const noexcept {return mFilePath;}
    ULONG lineNumber() const noexcept {return mLineNumber;}
    Log::Params params() const noexcept;

    Exception &operator=(const Exception &) = delete;

  private:
    const Subsystem &mSubsystem;
    String mMessage;
    CSTR mFunction;
    CSTR mFilePath;
    ULONG mLineNumber;
    Log::Params mParams;
  };
}

#define ZS_DECLARE_CUSTOM_EXCEPTION(xObject)                                                                    ZS_INTERNAL_DECLARE_CUSTOM_EXCEPTION(xObject)

#define ZS_DECLARE_CUSTOM_EXCEPTION_WITH_PROPERTIES_1(xObject, xType1, xName1)                                  ZS_INTERNAL_DECLARE_CUSTOM_EXCEPTION_WITH_PROPERTIES_1(xObject, xType1, xName1)
#define ZS_DECLARE_CUSTOM_EXCEPTION_WITH_PROPERTIES_2(xObject, xType1, xName1, xType2, xName2)                  ZS_INTERNAL_DECLARE_CUSTOM_EXCEPTION_WITH_PROPERTIES_2(xObject, xType1, xName1, xType2, xName2)
#define ZS_DECLARE_CUSTOM_EXCEPTION_WITH_PROPERTIES_3(xObject, xType1, xName1, xType2, xName2, xType3, xName3)  ZS_INTERNAL_DECLARE_CUSTOM_EXCEPTION_WITH_PROPERTIES_2(xObject, xType1, xName1, xType2, xName2, xType3, xName3)

#define ZS_DECLARE_CUSTOM_EXCEPTION_ALT_BASE(xObject, xBase)                                                    ZS_INTERNAL_DECLARE_CUSTOM_EXCEPTION_ALT_BASE(xObject, xBase)
#define ZS_DECLARE_CUSTOM_EXCEPTION_ALT_BASE_WITH_PROPERTIES_1(xObject, xBase, xType1)                          ZS_INTERNAL_DECLARE_CUSTOM_EXCEPTION_ALT_BASE_WITH_PROPERTIES_1(xObject, xBase, xType1)
#define ZS_DECLARE_CUSTOM_EXCEPTION_ALT_BASE_WITH_PROPERTIES_2(xObject, xBase, xType1, xType2)                  ZS_INTERNAL_DECLARE_CUSTOM_EXCEPTION_ALT_BASE_WITH_PROPERTIES_2(xObject, xBase, xType1, xType2)
#define ZS_DECLARE_CUSTOM_EXCEPTION_ALT_BASE_WITH_PROPERTIES_3(xObject, xBase, xType1, xType2, xType3)          ZS_INTERNAL_DECLARE_CUSTOM_EXCEPTION_ALT_BASE_WITH_PROPERTIES_3(xObject, xBase, xType1, xType2, xType3)

namespace zsLib
{
  namespace Exceptions
  {
    ZS_DECLARE_CUSTOM_EXCEPTION(InvalidArgument);
    ZS_DECLARE_CUSTOM_EXCEPTION(BadState);
    ZS_DECLARE_CUSTOM_EXCEPTION(SyntaxError);
    ZS_DECLARE_CUSTOM_EXCEPTION(RangeError);
    ZS_DECLARE_CUSTOM_EXCEPTION(ResourceError);
    ZS_DECLARE_CUSTOM_EXCEPTION(UnexpectedError);
    ZS_DECLARE_CUSTOM_EXCEPTION(InvalidUsage);
    ZS_DECLARE_CUSTOM_EXCEPTION(InvalidAssumption);
    ZS_DECLARE_CUSTOM_EXCEPTION(NotImplemented);
    ZS_DECLARE_CUSTOM_EXCEPTION(NotSupported);
    ZS_DECLARE_CUSTOM_EXCEPTION(InvalidModification);
    ZS_DECLARE_CUSTOM_EXCEPTION(NetworkError);
    ZS_DECLARE_CUSTOM_EXCEPTION(InternalError);
  };
} // namespace zsLib

#define ZS_THROW_INVALID_ARGUMENT(xMessage)                                                             {throw ::zsLib::Exceptions::InvalidArgument(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__);}
#define ZS_THROW_BAD_STATE(xMessage)                                                                    {throw ::zsLib::Exceptions::BadState(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__);}
#define ZS_THROW_RESOURCE_ERROR(xMessage)                                                               {throw ::zsLib::Exceptions::ResourceError(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__);}
#define ZS_THROW_UNEXPECTED_ERROR(xMessage)                                                             {throw ::zsLib::Exceptions::UnexpectedError(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__);}
#define ZS_THROW_INVALID_USAGE(xMessage)                                                                {throw ::zsLib::Exceptions::InvalidUsage(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__);}
#define ZS_THROW_INVALID_ASSUMPTION(xMessage)                                                           {throw ::zsLib::Exceptions::InvalidAssumption(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__);}
#define ZS_THROW_NOT_IMPLEMENTED(xMessage)                                                              {throw ::zsLib::Exceptions::NotImplemented(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__);}
#define ZS_THROW_NOT_SUPPORTED(xMessage)                                                                {throw ::zsLib::Exceptions::NotSupported(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__);}
#define ZS_THROW_SYNTAX_ERROR(xMessage)                                                                 {throw ::zsLib::Exceptions::SyntaxError(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__);}
#define ZS_THROW_RANGE_ERROR(xMessage)                                                                  {throw ::zsLib::Exceptions::RangeError(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__);}
#define ZS_THROW_INVALID_MODIFICATION_ERROR(xMessage)                                                   {throw ::zsLib::Exceptions::InvalidModification(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__);}
#define ZS_THROW_NETWORK_ERROR(xMessage)                                                                {throw ::zsLib::Exceptions::NetworkError(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__);}
#define ZS_THROW_INTERNAL_ERROR(xMessage)                                                               {throw ::zsLib::Exceptions::InternalError(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__);}
#define ZS_THROW_CUSTOM(xObject, xMessage)                                                              {throw xObject(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__);}
#define ZS_THROW_CUSTOM_PROPERTIES_1(xObject, xValue1, xMessage)                                        {throw xObject(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__, NULL, xValue1);}
#define ZS_THROW_CUSTOM_PROPERTIES_2(xObject, xValue1, xValue2, xMessage)                               {throw xObject(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__, NULL, xValue1, xValue2);}
#define ZS_THROW_CUSTOM_PROPERTIES_3(xObject, xValue1, xValue2, xValue3, xMessage)                      {throw xObject(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__, NULL, xValue1, xValue2, xValue3);}

#define ZS_THROW_INVALID_ARGUMENT_IF(xExperssion)                                                       {if (xExperssion) {throw ::zsLib::Exceptions::InvalidArgument(ZS_GET_SUBSYSTEM(), #xExperssion, __FUNCTION__, __FILE__, __LINE__);}}
#define ZS_THROW_BAD_STATE_IF(xExperssion)                                                              {if (xExperssion) {throw ::zsLib::Exceptions::BadState(ZS_GET_SUBSYSTEM(), #xExperssion, __FUNCTION__, __FILE__, __LINE__);}}
#define ZS_THROW_RESOURCE_ERROR_IF(xExperssion)                                                         {if (xExperssion) {throw ::zsLib::Exceptions::ResourceError(ZS_GET_SUBSYSTEM(), #xExperssion, __FUNCTION__, __FILE__, __LINE__);}}
#define ZS_THROW_UNEXPECTED_ERROR_IF(xExperssion)                                                       {if (xExperssion) {throw ::zsLib::Exceptions::UnexpectedError(ZS_GET_SUBSYSTEM(), #xExperssion, __FUNCTION__, __FILE__, __LINE__);}}
#define ZS_THROW_INVALID_USAGE_IF(xExperssion)                                                          {if (xExperssion) {throw ::zsLib::Exceptions::InvalidUsage(ZS_GET_SUBSYSTEM(), #xExperssion, __FUNCTION__, __FILE__, __LINE__);}}
#define ZS_THROW_INVALID_ASSUMPTION_IF(xExperssion)                                                     {if (xExperssion) {throw ::zsLib::Exceptions::InvalidAssumption(ZS_GET_SUBSYSTEM(), #xExperssion, __FUNCTION__, __FILE__, __LINE__);}}
#define ZS_THROW_NOT_IMPLEMENTED_IF(xExperssion)                                                        {if (xExperssion) {throw ::zsLib::Exceptions::NotImplemented(ZS_GET_SUBSYSTEM(), #xExperssion, __FUNCTION__, __FILE__, __LINE__);}}
#define ZS_THROW_NOT_SUPPORTED_IF(xExperssion)                                                          {if (xExperssion) {throw ::zsLib::Exceptions::NotSupported(ZS_GET_SUBSYSTEM(), #xExperssion, __FUNCTION__, __FILE__, __LINE__);}}
#define ZS_THROW_SYNTAX_ERROR_IF(xExperssion)                                                           {if (xExperssion) {throw ::zsLib::Exceptions::SyntaxError(ZS_GET_SUBSYSTEM(), #xExperssion, __FUNCTION__, __FILE__, __LINE__);}}
#define ZS_THROW_RANGE_ERROR_IF(xExperssion)                                                            {if (xExperssion) {throw ::zsLib::Exceptions::RangeError(ZS_GET_SUBSYSTEM(), #xExperssion, __FUNCTION__, __FILE__, __LINE__);}}
#define ZS_THROW_INVALID_MODIFICATION_IF(xExperssion)                                                   {if (xExperssion) {throw ::zsLib::Exceptions::InvalidModification(ZS_GET_SUBSYSTEM(), #xExperssion, __FUNCTION__, __FILE__, __LINE__);}}
#define ZS_THROW_NETWORK_ERROR_IF(xExperssion)                                                          {if (xExperssion) {throw ::zsLib::Exceptions::NetworkError(ZS_GET_SUBSYSTEM(), #xExperssion, __FUNCTION__, __FILE__, __LINE__);}}
#define ZS_THROW_INTERNAL_ERROR_IF(xExperssion)                                                         {if (xExperssion) {throw ::zsLib::Exceptions::InternalError(ZS_GET_SUBSYSTEM(), #xExperssion, __FUNCTION__, __FILE__, __LINE__);}}
#define ZS_THROW_CUSTOM_IF(xObject, xExperssion)                                                        {if (xExperssion) {throw xObject(ZS_GET_SUBSYSTEM(), #xExperssion, __FUNCTION__, __FILE__, __LINE__);}}
#define ZS_THROW_CUSTOM_PROPERTIES_1_IF(xObject, xExperssion, xValue1)                                  {if (xExperssion) {throw xObject(ZS_GET_SUBSYSTEM(), #xExperssion, __FUNCTION__, __FILE__, __LINE__, NULL, xValue1);}}
#define ZS_THROW_CUSTOM_PROPERTIES_2_IF(xObject, xExperssion, xValue1, xValue2)                         {if (xExperssion) {throw xObject(ZS_GET_SUBSYSTEM(), #xExperssion, __FUNCTION__, __FILE__, __LINE__, NULL, xValue1, xValue2);}}
#define ZS_THROW_CUSTOM_PROPERTIES_3_IF(xObject, xExperssion, xValue1, xValue2, xValue3)                {if (xExperssion) {throw xObject(ZS_GET_SUBSYSTEM(), #xExperssion, __FUNCTION__, __FILE__, __LINE__, NULL, xValue1, xValue2, xValue3);}}

#define ZS_THROW_INVALID_ARGUMENT_MSG_IF(xExperssion, xMessage)                                         {if (xExperssion) {throw ::zsLib::Exceptions::InvalidArgument(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__, #xExperssion);}}
#define ZS_THROW_BAD_STATE_MSG_IF(xExperssion, xMessage)                                                {if (xExperssion) {throw ::zsLib::Exceptions::BadState(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__, #xExperssion);}}
#define ZS_THROW_RESOURCE_ERROR_MSG_IF(xExperssion, xMessage)                                           {if (xExperssion) {throw ::zsLib::Exceptions::ResourceError(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__, #xExperssion);}}
#define ZS_THROW_UNEXPECTED_ERROR_MSG_IF(xExperssion, xMessage)                                         {if (xExperssion) {throw ::zsLib::Exceptions::UnexpectedError(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__, #xExperssion);}}
#define ZS_THROW_INVALID_USAGE_MSG_IF(xExperssion, xMessage)                                            {if (xExperssion) {throw ::zsLib::Exceptions::InvalidUsage(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__, #xExperssion);}}
#define ZS_THROW_INVALID_ASSUMPTION_MSG_IF(xExperssion, xMessage)                                       {if (xExperssion) {throw ::zsLib::Exceptions::InvalidAssumption(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__, #xExperssion);}}
#define ZS_THROW_NOT_IMPLEMENTED_MSG_IF(xExperssion, xMessage)                                          {if (xExperssion) {throw ::zsLib::Exceptions::NotImplemented(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__, #xExperssion);}}
#define ZS_THROW_NOT_SUPPORTED_MSG_IF(xExperssion, xMessage)                                            {if (xExperssion) {throw ::zsLib::Exceptions::NotSupported(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__, #xExperssion);}}
#define ZS_THROW_SYNTAX_ERROR_MSG_IF(xExperssion, xMessage)                                             {if (xExperssion) {throw ::zsLib::Exceptions::SyntaxError(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__, #xExperssion);}}
#define ZS_THROW_RANGE_ERROR_MSG_IF(xExperssion, xMessage)                                              {if (xExperssion) {throw ::zsLib::Exceptions::RangeError(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__, #xExperssion);}}
#define ZS_THROW_INVALID_MODIFICATION_MSG_IF(xExperssion, xMessage)                                     {if (xExperssion) {throw ::zsLib::Exceptions::InvalidModification(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__, #xExperssion);}}
#define ZS_THROW_NETWORK_ERROR_MSG_IF(xExperssion, xMessage)                                            {if (xExperssion) {throw ::zsLib::Exceptions::NetworkError(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__, #xExperssion);}}
#define ZS_THROW_INTERNAL_ERROR_MSG_IF(xExperssion, xMessage)                                           {if (xExperssion) {throw ::zsLib::Exceptions::InternalError(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__, #xExperssion);}}
#define ZS_THROW_CUSTOM_MSG_IF(xObject, xExperssion, xMessage)                                          {if (xExperssion) {throw xObject(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__, #xExperssion);}}
#define ZS_THROW_CUSTOM_MSG_PROPERTIES_1_IF(xObject, xExperssion, xValue1, xMessage)                    {if (xExperssion) {throw xObject(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__, #xExperssion, xValue1);}}
#define ZS_THROW_CUSTOM_MSG_PROPERTIES_2_IF(xObject, xExperssion, xValue1, xValue2, xMessage)           {if (xExperssion) {throw xObject(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__, #xExperssion, xValue1, xValue2);}}
#define ZS_THROW_CUSTOM_MSG_PROPERTIES_3_IF(xObject, xExperssion, xValue1, xValue2, xValue3, xMessage)  {if (xExperssion) {throw xObject(ZS_GET_SUBSYSTEM(), xMessage, __FUNCTION__, __FILE__, __LINE__, #xExperssion, xValue1, xValue2, xValue3);}}
