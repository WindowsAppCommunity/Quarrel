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

#ifdef DEBUG
#ifndef _DEBUG
#define _DEBUG 1
#endif //_DEBUG
#endif //DEBUG

#ifdef _DEBUG
#ifndef DEBUG
#define DEBUG 1
#endif //DEBUG
#endif //_DEBUG

 // see https://stackoverflow.com/questions/2124339/c-preprocessor-va-args-number-of-arguments

#ifdef _MSC_VER // Microsoft compilers

#   define ZS_INTERNAL_MACRO_GET_ARG_ACOUNT(...)  ZS_INTERNAL_EXPAND_ARGS_PRIVATE(ZS_INTERNAL_ARGS_AUGMENTER(__VA_ARGS__))
#   define ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_1(...)  ZS_INTERNAL_EXPAND_ARGS_PRIVATE_MINUS_1(ZS_INTERNAL_ARGS_AUGMENTER(__VA_ARGS__))
#   define ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_2(...)  ZS_INTERNAL_EXPAND_ARGS_PRIVATE_MINUS_2(ZS_INTERNAL_ARGS_AUGMENTER(__VA_ARGS__))
#   define ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_3(...)  ZS_INTERNAL_EXPAND_ARGS_PRIVATE_MINUS_3(ZS_INTERNAL_ARGS_AUGMENTER(__VA_ARGS__))
#   define ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_4(...)  ZS_INTERNAL_EXPAND_ARGS_PRIVATE_MINUS_4(ZS_INTERNAL_ARGS_AUGMENTER(__VA_ARGS__))
#   define ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_5(...)  ZS_INTERNAL_EXPAND_ARGS_PRIVATE_MINUS_5(ZS_INTERNAL_ARGS_AUGMENTER(__VA_ARGS__))
#   define ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6(...)  ZS_INTERNAL_EXPAND_ARGS_PRIVATE_MINUS_6(ZS_INTERNAL_ARGS_AUGMENTER(__VA_ARGS__))

#   define ZS_INTERNAL_ARGS_AUGMENTER(...) unused, __VA_ARGS__
#   define ZS_INTERNAL_EXPAND(x) x
#   define ZS_INTERNAL_EXPAND_ARGS_PRIVATE(...) ZS_INTERNAL_EXPAND(ZS_INTERNAL_GET_ARG_COUNT_PRIVATE(__VA_ARGS__, 69, 68, 67, 66, 65, 64, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0))
#   define ZS_INTERNAL_GET_ARG_COUNT_PRIVATE(_1_, _2_, _3_, _4_, _5_, _6_, _7_, _8_, _9_, _10_, _11_, _12_, _13_, _14_, _15_, _16_, _17_, _18_, _19_, _20_, _21_, _22_, _23_, _24_, _25_, _26_, _27_, _28_, _29_, _30_, _31_, _32_, _33_, _34_, _35_, _36, _37, _38, _39, _40, _41, _42, _43, _44, _45, _46, _47, _48, _49, _50, _51, _52, _53, _54, _55, _56, _57, _58, _59, _60, _61, _62, _63, _64, _65, _66, _67, _68, _69, _70, count, ...) count
#   define ZS_INTERNAL_EXPAND_ARGS_PRIVATE_MINUS_1(...) ZS_INTERNAL_EXPAND(ZS_INTERNAL_GET_ARG_COUNT_PRIVATE(__VA_ARGS__, 68, 67, 66, 65, 64, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 0))
#   define ZS_INTERNAL_EXPAND_ARGS_PRIVATE_MINUS_2(...) ZS_INTERNAL_EXPAND(ZS_INTERNAL_GET_ARG_COUNT_PRIVATE(__VA_ARGS__, 67, 66, 65, 64, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 0, 0))
#   define ZS_INTERNAL_EXPAND_ARGS_PRIVATE_MINUS_3(...) ZS_INTERNAL_EXPAND(ZS_INTERNAL_GET_ARG_COUNT_PRIVATE(__VA_ARGS__, 66, 65, 64, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 0, 0, 0))
#   define ZS_INTERNAL_EXPAND_ARGS_PRIVATE_MINUS_4(...) ZS_INTERNAL_EXPAND(ZS_INTERNAL_GET_ARG_COUNT_PRIVATE(__VA_ARGS__, 65, 64, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 0, 0, 0, 0))
#   define ZS_INTERNAL_EXPAND_ARGS_PRIVATE_MINUS_5(...) ZS_INTERNAL_EXPAND(ZS_INTERNAL_GET_ARG_COUNT_PRIVATE(__VA_ARGS__, 64, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 0, 0, 0, 0, 0))
#   define ZS_INTERNAL_EXPAND_ARGS_PRIVATE_MINUS_6(...) ZS_INTERNAL_EXPAND(ZS_INTERNAL_GET_ARG_COUNT_PRIVATE(__VA_ARGS__, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 0, 0, 0, 0, 0, 0))

#else // _MSC_VER

#   define ZS_INTERNAL_MACRO_GET_ARG_ACOUNT(...) ZS_INTERNAL_GET_ARG_COUNT_PRIVATE(0, ## __VA_ARGS__, 70, 69, 68, 67, 66, 65, 64, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0)
#   define ZS_INTERNAL_GET_ARG_COUNT_PRIVATE(_0, _1_, _2_, _3_, _4_, _5_, _6_, _7_, _8_, _9_, _10_, _11_, _12_, _13_, _14_, _15_, _16_, _17_, _18_, _19_, _20_, _21_, _22_, _23_, _24_, _25_, _26_, _27_, _28_, _29_, _30_, _31_, _32_, _33_, _34_, _35_, _36, _37, _38, _39, _40, _41, _42, _43, _44, _45, _46, _47, _48, _49, _50, _51, _52, _53, _54, _55, _56, _57, _58, _59, _60, _61, _62, _63, _64, _65, _66, _67, _68, _69, _70, count, ...) count

#   define ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_1(...) ZS_INTERNAL_GET_ARG_COUNT_PRIVATE(0, ## __VA_ARGS__, 69, 68, 67, 66, 65, 64, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 0)
#   define ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_2(...) ZS_INTERNAL_GET_ARG_COUNT_PRIVATE(0, ## __VA_ARGS__, 68, 67, 66, 65, 64, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 0, 0)
#   define ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_3(...) ZS_INTERNAL_GET_ARG_COUNT_PRIVATE(0, ## __VA_ARGS__, 67, 66, 65, 64, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 0, 0, 0)
#   define ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_4(...) ZS_INTERNAL_GET_ARG_COUNT_PRIVATE(0, ## __VA_ARGS__, 66, 65, 64, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 0, 0, 0, 0)
#   define ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_5(...) ZS_INTERNAL_GET_ARG_COUNT_PRIVATE(0, ## __VA_ARGS__, 65, 64, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 0, 0, 0, 0, 0)
#   define ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6(...) ZS_INTERNAL_GET_ARG_COUNT_PRIVATE(0, ## __VA_ARGS__, 64, 63, 62, 61, 60, 59, 58, 57, 56, 55, 54, 53, 52, 51, 50, 49, 48, 47, 46, 45, 44, 43, 42, 41, 40, 39, 38, 37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 0, 0, 0, 0, 0, 0)

#endif // _MSC_VER

#ifdef _DEBUG

#define ZS_INTERNAL_VERIFY_INDIRECT_MINUS_1(...) ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_1(__VA_ARGS__)

static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT() == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT() failed for 0 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT(1) == 1, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT() failed for 1 argument");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT(1, 2) == 2, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT() failed for 2 arguments");

static_assert(ZS_INTERNAL_VERIFY_INDIRECT_MINUS_1() == 0, "ZS_INTERNAL_VERIFY_INDIRECT_MINUS_1() failed for 0 arguments");
static_assert(ZS_INTERNAL_VERIFY_INDIRECT_MINUS_1(x) == 0, "ZS_INTERNAL_VERIFY_INDIRECT_MINUS_1() failed for 1 arguments");
static_assert(ZS_INTERNAL_VERIFY_INDIRECT_MINUS_1(x, 1) == 1, "ZS_INTERNAL_VERIFY_INDIRECT_MINUS_1() failed for 2 argument");
static_assert(ZS_INTERNAL_VERIFY_INDIRECT_MINUS_1(x, 1, 2) == 2, "ZS_INTERNAL_VERIFY_INDIRECT_MINUS_1() failed for 3 arguments");

static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_2() == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_2() failed for 0 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_2(x) == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_2() failed for 1 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_2(x, 1) == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_2() failed for 2 argument");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_2(x, 1, 2) == 1, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_2() failed for 3 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_2(x, 1, 2, 3) == 2, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_2() failed for 4 arguments");

static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_3() == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_3() failed for 0 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_3(x) == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_3() failed for 1 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_3(x, 1) == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_3() failed for 2 argument");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_3(x, 1, 2) == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_3() failed for 3 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_3(x, 1, 2, 3) == 1, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_3() failed for 4 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_3(x, 1, 2, 3, 4) == 2, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_3() failed for 5 arguments");

static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_4() == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_4() failed for 0 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_4(x) == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_4() failed for 1 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_4(x, 1) == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_4() failed for 2 argument");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_4(x, 1, 2) == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_4() failed for 3 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_4(x, 1, 2, 3) == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_4() failed for 4 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_4(x, 1, 2, 3, 4) == 1, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_4() failed for 5 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_4(x, 1, 2, 3, 4, 5) == 2, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_4() failed for 5 arguments");

static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_5() == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_5() failed for 0 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_5(x) == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_5() failed for 1 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_5(x, 1) == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_5() failed for 2 argument");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_5(x, 1, 2) == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_5() failed for 3 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_5(x, 1, 2, 3) == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_5() failed for 4 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_5(x, 1, 2, 3, 4) == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_5() failed for 5 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_5(x, 1, 2, 3, 4, 5) == 1, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_5() failed for 5 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_5(x, 1, 2, 3, 4, 5, 6) == 2, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_5() failed for 5 arguments");

static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6() == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6() failed for 0 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6(x) == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6() failed for 1 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6(x, 1) == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6() failed for 2 argument");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6(x, 1, 2) == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6() failed for 3 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6(x, 1, 2, 3) == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6() failed for 4 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6(x, 1, 2, 3, 4) == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6() failed for 5 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6(x, 1, 2, 3, 4, 5) == 0, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6() failed for 5 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6(x, 1, 2, 3, 4, 5, 6) == 1, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6() failed for 5 arguments");
static_assert(ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6(x, 1, 2, 3, 4, 5, 6, 7) == 2, "ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6() failed for 5 arguments");

#endif //_DEBUG

// see https://stackoverflow.com/questions/2124339/c-preprocessor-va-args-number-of-arguments
#define ZS_INTERNAL_MACRO_CAT( A, B ) A ## B
#define ZS_INTERNAL_MACRO_VA_SELECT( NAME, NUM ) ZS_INTERNAL_MACRO_CAT( NAME ## _, NUM )

#define ZS_INTERNAL_MACRO_SELECT( NAME, ... )                ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT(__VA_ARGS__) )(__VA_ARGS__)

#ifdef _MSC_VER // Microsoft compilers

// see https://stackoverflow.com/questions/5134523/msvc-doesnt-expand-va-args-correctly

#define ZS_MACRO_SELECT_WITH_PROPERTY_1(NAME, ...)            ZS_INTERNAL_EXPAND(ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_1(__VA_ARGS__) )(__VA_ARGS__))
#define ZS_MACRO_SELECT_WITH_PROPERTY_2(NAME, ...)            ZS_INTERNAL_EXPAND(ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_2(__VA_ARGS__) )(__VA_ARGS__))
#define ZS_MACRO_SELECT_WITH_PROPERTY_3(NAME, ...)            ZS_INTERNAL_EXPAND(ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_3(__VA_ARGS__) )(__VA_ARGS__))
#define ZS_MACRO_SELECT_WITH_PROPERTY_4(NAME, ...)            ZS_INTERNAL_EXPAND(ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_4(__VA_ARGS__) )(__VA_ARGS__))
#define ZS_MACRO_SELECT_WITH_PROPERTY_5(NAME, ...)            ZS_INTERNAL_EXPAND(ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_5(__VA_ARGS__) )(__VA_ARGS__))
#define ZS_MACRO_SELECT_WITH_PROPERTY_6(NAME, ...)            ZS_INTERNAL_EXPAND(ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6(__VA_ARGS__) )(__VA_ARGS__))

#define ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_1(NAME, ...)   ZS_INTERNAL_EXPAND(ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_1(__VA_ARGS__) )(__VA_ARGS__))
#define ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_2(NAME, ...)   ZS_INTERNAL_EXPAND(ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_2(__VA_ARGS__) )(__VA_ARGS__))
#define ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_3(NAME, ...)   ZS_INTERNAL_EXPAND(ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_3(__VA_ARGS__) )(__VA_ARGS__))
#define ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_4(NAME, ...)   ZS_INTERNAL_EXPAND(ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_4(__VA_ARGS__) )(__VA_ARGS__))
#define ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_5(NAME, ...)   ZS_INTERNAL_EXPAND(ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_5(__VA_ARGS__) )(__VA_ARGS__))
#define ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_6(NAME, ...)   ZS_INTERNAL_EXPAND(ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6(__VA_ARGS__) )(__VA_ARGS__))

#else

#define ZS_MACRO_SELECT_WITH_PROPERTY_1(NAME, ...)            ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_1(__VA_ARGS__) )(__VA_ARGS__)
#define ZS_MACRO_SELECT_WITH_PROPERTY_2(NAME, ...)            ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_2(__VA_ARGS__) )(__VA_ARGS__)
#define ZS_MACRO_SELECT_WITH_PROPERTY_3(NAME, ...)            ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_3(__VA_ARGS__) )(__VA_ARGS__)
#define ZS_MACRO_SELECT_WITH_PROPERTY_4(NAME, ...)            ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_4(__VA_ARGS__) )(__VA_ARGS__)
#define ZS_MACRO_SELECT_WITH_PROPERTY_5(NAME, ...)            ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_5(__VA_ARGS__) )(__VA_ARGS__)
#define ZS_MACRO_SELECT_WITH_PROPERTY_6(NAME, ...)            ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6(__VA_ARGS__) )(__VA_ARGS__)

#define ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_1(NAME, ...)   ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_1(__VA_ARGS__) )(__VA_ARGS__)
#define ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_2(NAME, ...)   ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_2(__VA_ARGS__) )(__VA_ARGS__)
#define ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_3(NAME, ...)   ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_3(__VA_ARGS__) )(__VA_ARGS__)
#define ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_4(NAME, ...)   ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_4(__VA_ARGS__) )(__VA_ARGS__)
#define ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_5(NAME, ...)   ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_5(__VA_ARGS__) )(__VA_ARGS__)
#define ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_6(NAME, ...)   ZS_INTERNAL_MACRO_VA_SELECT( NAME, ZS_INTERNAL_MACRO_GET_ARG_ACOUNT_MINUS_6(__VA_ARGS__) )(__VA_ARGS__)

#endif //_MSC_VER

#ifdef _DEBUG
#define ZS_INTERNAL_VERIFY_CHOICE_A_0(x)                    (8192 + x)
#define ZS_INTERNAL_VERIFY_CHOICE_A_1(x, a)                 (8192 + x + a)
#define ZS_INTERNAL_VERIFY_CHOICE_A_2(x, a, b)              (8192 + x + a + b)
#define ZS_INTERNAL_VERIFY_CHOICE_A_3(x, a, b, c)           (8192 + x + a + b + c)

#define ZS_INTERNAL_VERIFY_CHOICE_B_0(x1, x2)               (1024 + (x1*3) + (x2*2))
#define ZS_INTERNAL_VERIFY_CHOICE_B_1(x1, x2, a)            (1024 + (x1*3) + (x2*2) + a)
#define ZS_INTERNAL_VERIFY_CHOICE_B_2(x1, x2, a, b)         (1024 + (x1*3) + (x2*2) + a + b)
#define ZS_INTERNAL_VERIFY_CHOICE_B_3(x1, x2, a, b, c)      (1024 + (x1*3) + (x2*2) + a + b + c)

#define ZS_INTERNAL_VERIFY_CHOICE_C_0(x1, x2, x3)           (2048 + (x1*2) + (x2*3) + (x3*4))
#define ZS_INTERNAL_VERIFY_CHOICE_C_1(x1, x2, x3, a)        (2048 + (x1*2) + (x2*3) + (x3*4) + a)
#define ZS_INTERNAL_VERIFY_CHOICE_C_2(x1, x2, x3, a, b)     (2048 + (x1*2) + (x2*3) + (x3*4) + a + b)
#define ZS_INTERNAL_VERIFY_CHOICE_C_3(x1, x2, x3, a, b, c)  (2048 + (x1*2) + (x2*3) + (x3*4) + a + b + c)

#define ZS_INTERNAL_VERIFY_CHOICE_D_0(x1, x2, x3, x4)             (4096 + (x1*2) + (x2*3) + (x3*4) + (x4*5))
#define ZS_INTERNAL_VERIFY_CHOICE_D_1(x1, x2, x3, x4, a)          (4096 + (x1*2) + (x2*3) + (x3*4) + (x4*5) + a)
#define ZS_INTERNAL_VERIFY_CHOICE_D_2(x1, x2, x3, x4, a, b)       (4096 + (x1*2) + (x2*3) + (x3*4) + (x4*5) + a + b)
#define ZS_INTERNAL_VERIFY_CHOICE_D_3(x1, x2, x3, x4, a, b, c)    (4096 + (x1*2) + (x2*3) + (x3*4) + (x4*5) + a + b + c)

#define ZS_INTERNAL_VERIFY_CHOICE_E_0(x1, x2, x3, x4, x5)             (512 + (x1*2) + (x2*3) + (x3*4) + (x4*5) + (x5*6))
#define ZS_INTERNAL_VERIFY_CHOICE_E_1(x1, x2, x3, x4, x5, a)          (512 + (x1*2) + (x2*3) + (x3*4) + (x4*5) + (x5*6) + a)
#define ZS_INTERNAL_VERIFY_CHOICE_E_2(x1, x2, x3, x4, x5, a, b)       (512 + (x1*2) + (x2*3) + (x3*4) + (x4*5) + (x5*6) + a + b)
#define ZS_INTERNAL_VERIFY_CHOICE_E_3(x1, x2, x3, x4, x5, a, b, c)    (512 + (x1*2) + (x2*3) + (x3*4) + (x4*5) + (x5*6) + a + b + c)

static_assert(ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_1(ZS_INTERNAL_VERIFY_CHOICE_A, 1000) == 9192, "ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_1() failed for 1 property and 0 arguments");
static_assert(ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_1(ZS_INTERNAL_VERIFY_CHOICE_A, 1000, 1) == 9193, "ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_1() failed for 1 property and 1 argument");
static_assert(ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_1(ZS_INTERNAL_VERIFY_CHOICE_A, 1000, 1, 2) == 9195, "ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_1() failed for 1 property and 2 arguments");
static_assert(ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_1(ZS_INTERNAL_VERIFY_CHOICE_A, 1000, 1, 2, 3) == 9198, "ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_1() failed for 1 property and 3 arguments");

static_assert(ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_2(ZS_INTERNAL_VERIFY_CHOICE_B, 100, 1000) == 3324, "ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_2() failed for 2 properties and 0 arguments");
static_assert(ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_2(ZS_INTERNAL_VERIFY_CHOICE_B, 100, 1000, 1) == 3325, "ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_2() failed for 2 properties and 1 argument");
static_assert(ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_2(ZS_INTERNAL_VERIFY_CHOICE_B, 100, 1000, 1, 2) == 3327, "ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_2() failed for 2 properties and 2 arguments");
static_assert(ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_2(ZS_INTERNAL_VERIFY_CHOICE_B, 100, 1000, 1, 2, 3) == 3330, "ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_2() failed for 2 properties and 3 arguments");

static_assert(ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_3(ZS_INTERNAL_VERIFY_CHOICE_C, 100, 1000, 10000) == 45248, "ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_3() failed for 3 properties and 0 arguments");
static_assert(ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_3(ZS_INTERNAL_VERIFY_CHOICE_C, 100, 1000, 10000, 1) == 45249, "ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_3() failed for 3 properties and 1 argument");
static_assert(ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_3(ZS_INTERNAL_VERIFY_CHOICE_C, 100, 1000, 10000, 1, 2) == 45251, "ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_3() failed for 3 properties and 2 arguments");
static_assert(ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_3(ZS_INTERNAL_VERIFY_CHOICE_C, 100, 1000, 10000, 1, 2, 3) == 45254, "ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_3() failed for 3 properties and 3 arguments");

static_assert(ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_4(ZS_INTERNAL_VERIFY_CHOICE_D, 100, 1000, 10000, 100000) == 547296, "ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_4() failed for 4 properties and 0 arguments");
static_assert(ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_4(ZS_INTERNAL_VERIFY_CHOICE_D, 100, 1000, 10000, 100000, 1) == 547297, "ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_4() failed for 4 properties and 1 argument");
static_assert(ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_4(ZS_INTERNAL_VERIFY_CHOICE_D, 100, 1000, 10000, 100000, 1, 2) == 547299, "ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_4() failed for 4 properties and 2 arguments");
static_assert(ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_4(ZS_INTERNAL_VERIFY_CHOICE_D, 100, 1000, 10000, 100000, 1, 2, 3) == 547302, "ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_4() failed for 4 properties and 3 arguments");

static_assert(ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_5(ZS_INTERNAL_VERIFY_CHOICE_E, 100, 1000, 10000, 100000, 1000000) == 6543712, "ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_5() failed for 5 properties and 0 arguments");
static_assert(ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_5(ZS_INTERNAL_VERIFY_CHOICE_E, 100, 1000, 10000, 100000, 1000000, 1) == 6543713, "ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_5() failed for 5 properties and 1 argument");
static_assert(ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_5(ZS_INTERNAL_VERIFY_CHOICE_E, 100, 1000, 10000, 100000, 1000000, 1, 2) == 6543715, "ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_5() failed for 5 properties and 2 arguments");
static_assert(ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_5(ZS_INTERNAL_VERIFY_CHOICE_E, 100, 1000, 10000, 100000, 1000000, 1, 2, 3) == 6543718, "ZS_INTERNAL_MACRO_SELECT_WITH_PROPERTY_5() failed for 5 properties and 3 arguments");

#endif //_DEBUG

#if __cplusplus >= 201703L // || ((defined(_MSC_VER ) && __cplusplus > 201703L))
// If you are having trouble with Visual Studio c++17 and this macro please see:
// https://blogs.msdn.microsoft.com/vcblog/2018/04/09/msvc-now-correctly-reports-__cplusplus/
// NOTE: specify /Zc:__cplusplus for c++17 on MSVC
#define ZS_INTERNAL_MAYBE_USED_0() [[maybe_unused]]
#define ZS_INTERNAL_NO_DISCARD() [[nodiscard]]
#else
#define ZS_INTERNAL_MAYBE_USED_0() 
#define ZS_INTERNAL_NO_DISCARD()
#endif //__cplusplus >= 201703L

// https://stackoverflow.com/questions/5966594/how-can-i-use-pragma-message-so-that-the-message-points-to-the-filelineno

#define ZS_INTERNAL_QUOTED_LINE_STRINGIZER( L )     #L 
#define ZS_INTERNAL_MAKE_QUOTED_LINE_STRING( M, L ) M(L)
#define ZS_INTERNAL_QUOTED_LINE() ZS_INTERNAL_MAKE_QUOTED_LINE_STRING( ZS_INTERNAL_QUOTED_LINE_STRINGIZER, __LINE__ )
#define ZS_INTERNAL_FILE_LINE() __FILE__ "(" ZS_INTERNAL_QUOTED_LINE() ") : "


#define ZS_INTERNAL_BUILD_NOTE(xFieldName, xMsg)                                                               message(ZS_INTERNAL_FILE_LINE() "\n" \
  "------------------------------------------------------------------------------\n"                                                                \
  "| " xFieldName " -> " xMsg "\n"                                                                                                            \
  "------------------------------------------------------------------------------" )


#define ZS_INTERNAL_MAYBE_USED_1(xVariable) ((void)xVariable);

#define ZS_INTERNAL_MAYBE_USED(...) ZS_INTERNAL_MACRO_SELECT(ZS_INTERNAL_MAYBE_USED, __VA_ARGS__)

#define ZS_INTERNAL_ASSERT(xCondition) assert(xCondition);
#define ZS_INTERNAL_ASSERT_MESSAGE(xCondition, xMsg) assert((xCondition) && xMsg);
#define ZS_INTERNAL_ASSERT_FAIL(xMsg) assert(false && xMsg);

#ifdef _WIN32

#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif //WIN32_LEAN_AND_MEAN
#include <winsock2.h>
#include <rpc.h>
#include <stdint.h>
#endif //_WIN32

#include <assert.h>
#include <atomic>
#include <limits.h>
#include <chrono>
#include <thread>
#include <mutex>
#ifndef _WIN32
#include <cstring>
#include <uuid/uuid.h>
#endif //_WIN32

//#ifndef interface
//#define interface struct
//#endif //interface

#ifndef interaction
#define interaction struct
#endif //interaction

#include <memory>

#ifdef __has_include
#if (__has_include(<optional>)) && (__cplusplus >= 201703L)
#include <optional>
#define ZS_HAS_STD_OPTIONAL
#endif //(__has_include(<optional>)) && (__cplusplus >= 201703L)
#endif //__has_include

#define ZS_INTERNAL_DECLARE_PTR(xExistingType)                                                      \
  typedef std::shared_ptr<xExistingType> xExistingType##Ptr;                                        \
  typedef std::weak_ptr<xExistingType> xExistingType##WeakPtr;                                      \
  typedef std::unique_ptr<xExistingType> xExistingType##UniPtr;

#define ZS_INTERNAL_DECLARE_USING_PTR(xNamespace, xExistingType)                                    \
  using xNamespace::xExistingType;                                                                  \
  using xNamespace::xExistingType##Ptr;                                                             \
  using xNamespace::xExistingType##WeakPtr;                                                         \
  using xNamespace::xExistingType##UniPtr;

#define ZS_INTERNAL_DECLARE_CLASS_PTR(xClassName)                                                   \
  class xClassName;                                                                                 \
  typedef std::shared_ptr<xClassName> xClassName##Ptr;                                              \
  typedef std::weak_ptr<xClassName> xClassName##WeakPtr;                                            \
  typedef std::unique_ptr<xClassName> xClassName##UniPtr;

#define ZS_INTERNAL_DECLARE_STRUCT_PTR(xStructName)                                                 \
  struct xStructName;                                                                               \
  typedef std::shared_ptr<xStructName> xStructName##Ptr;                                            \
  typedef std::weak_ptr<xStructName> xStructName##WeakPtr;                                          \
  typedef std::unique_ptr<xStructName> xStructName##UniPtr;

#define ZS_INTERNAL_DECLARE_TYPEDEF_PTR(xOriginalType, xNewTypeName)                                \
  typedef xOriginalType xNewTypeName;                                                               \
  typedef std::shared_ptr<xNewTypeName> xNewTypeName##Ptr;                                          \
  typedef std::weak_ptr<xNewTypeName> xNewTypeName##WeakPtr;                                        \
  typedef std::unique_ptr<xNewTypeName> xNewTypeName##UniPtr;

#define ZS_INTERNAL_DYNAMIC_PTR_CAST(xType, xObject)                                                \
  std::dynamic_pointer_cast<xType>(xObject)

#ifdef _WIN32
namespace std
{
	typedef intmax_t intmax_t;
	typedef uintmax_t uintmax_t;
}
#define alignof(xValue) __alignof(xValue)

#if ULLONG_MAX == 0xFFFFFFFFFFFFFFFF
typedef ULONGLONG QWORD;

#else //ULLONG_MAX == 0xFFFFFFFFFFFFFFFF

#if ULONG_MAX == 0xFFFFFFFFFFFFFFFF
typedef ULONG QWORD;
#endif //ULONG_MAX == 0xFFFFFFFFFFFFFFFF

#endif //ULONG_MAX == 0xFFFFFFFFFFFFFFFF

#endif //_WIN32

namespace zsLib
{
#ifdef _WIN32
  using ::CHAR;
  using ::UCHAR;

  using ::SHORT;
  using ::USHORT;

  using ::INT;
  using ::UINT;

  using ::LONG;
  using ::ULONG;

  using ::LONGLONG;
  using ::ULONGLONG;

  using ::BYTE;
  using ::WORD;
  using ::DWORD;
  using ::QWORD;
  
  typedef SSIZE_T ssize_t;

#else
  typedef char CHAR;
  typedef unsigned char UCHAR;

  typedef short SHORT;
  typedef unsigned short USHORT;

  typedef int INT;
  typedef unsigned int UINT;

  typedef long LONG;
  typedef unsigned long ULONG;

  typedef long long LONGLONG;
  typedef unsigned long long ULONGLONG;

#if UCHAR_MAX == 0xFF
  typedef UCHAR BYTE;
#endif //UCHAR_MAX == 0xFF

#if USHRT_MAX == 0xFFFF
  typedef USHORT WORD;
#endif

#if UINT_MAX == 0xFFFFFFFF
  typedef UINT DWORD;
#endif //UINT_MAX == 0xFFFFFFFF

#if ULONG_MAX == 0xFFFFFFFFFFFFFFFF
  typedef ULONG QWORD;

#else //ULONG_MAX == 0xFFFFFFFFFFFFFFFF

#if ULLONG_MAX == 0xFFFFFFFFFFFFFFFF
  typedef ULONGLONG QWORD;
#endif

#endif //ULONG_MAX == 0xFFFFFFFFFFFFFFFF

#endif //_WIN32

  typedef std::intmax_t LONGEST;
  typedef std::uintmax_t ULONGEST;

  typedef float FLOAT;
  typedef double DOUBLE;

  typedef ULONG PUID;

#if defined(__LP64__) || defined(_WIN64) || defined(_Wp64)
#define ZSLIB_64BIT
#else
#define ZSLIB_32BIT
#endif //defined(__LP64__) || defined(_WIN64) || defined(_Wp64)

  typedef uintptr_t PTRNUMBER;
  typedef PTRNUMBER USERPARAM;

  namespace internal
  {
    struct uuid_wrapper
    {
		  typedef UCHAR * iterator;
		  typedef UCHAR const* const_iterator;

#ifndef _WIN32
      typedef uuid_t raw_uuid_type;
	    raw_uuid_type mUUID{};

	    iterator begin() { return mUUID; }
	    const_iterator begin() const { return mUUID; }
	    iterator end() { return mUUID + sizeof(raw_uuid_type); }
	    const_iterator end() const { return mUUID + sizeof(raw_uuid_type); }
      size_t size() const { return sizeof(raw_uuid_type); }
#else
	    typedef GUID raw_uuid_type;
	    raw_uuid_type mUUID {};

	    iterator begin() { return (iterator) (&mUUID); }
	    const_iterator begin() const { return (iterator)(&mUUID); }
	    iterator end() { return begin() + sizeof(raw_uuid_type); }
	    const_iterator end() const { return begin() + sizeof(raw_uuid_type); }
      size_t size() const { return sizeof(raw_uuid_type); }

	    static void uuid_clear(raw_uuid_type &uuid)
      {
		    memset(&uuid, 0, sizeof(uuid));
	    }

	    static void uuid_copy(raw_uuid_type &dest, const raw_uuid_type &source)
      {
		    memcpy(&dest, &source, sizeof(dest));
	    }

	    static int uuid_compare(const raw_uuid_type &op1, const raw_uuid_type &op2)
      {
		    return memcmp(&op1, &op2, sizeof(op1));
	    }

	    static bool uuid_is_null(const raw_uuid_type &op)
      {
        struct ClearUUID
        {
          ClearUUID() {uuid_clear(mEmpty);}
        
          const raw_uuid_type &value() const {return mEmpty;}
          raw_uuid_type mEmpty;
        };
        static ClearUUID emptyUUID;
		    return 0 == memcmp(&op, &(emptyUUID.value()), sizeof(op));
	    }
#endif //ndef _WIN32

      uuid_wrapper() {
        uuid_clear(mUUID);
      }

      uuid_wrapper(const uuid_wrapper &op2) {
        uuid_copy(mUUID, op2.mUUID);
      }

      int compare(const uuid_wrapper &op2) const {
        return uuid_compare(mUUID, op2.mUUID);
      }

      bool operator!() const {
        return uuid_is_null(mUUID);
      }

      uuid_wrapper &operator=(const uuid_wrapper &op2) {
        uuid_copy(mUUID, op2.mUUID);
        return *this;
      }

      bool operator==(const uuid_wrapper &op2) const {
        return 0 == uuid_compare(mUUID, op2.mUUID);
      }
      bool operator!=(const uuid_wrapper &op2) const {
        return 0 != uuid_compare(mUUID, op2.mUUID);
      }
      bool operator<(const uuid_wrapper &op2) const {
        return uuid_compare(mUUID, op2.mUUID) < 0;
      }
      bool operator>(const uuid_wrapper &op2) const {
        return uuid_compare(mUUID, op2.mUUID) > 0;
      }
      bool operator<=(const uuid_wrapper &op2) const {
        return uuid_compare(mUUID, op2.mUUID) <= 0;
      }
      bool operator>=(const uuid_wrapper &op2) const {
        return uuid_compare(mUUID, op2.mUUID) >= 0;
      }
    };
  }

  typedef internal::uuid_wrapper UUID;
  
  typedef char CHAR;
  typedef wchar_t WCHAR;

#ifdef UNICODE
  typedef WCHAR TCHAR;
#else
  typedef CHAR TCHAR;
#endif //UNICODE


#if (__WCHAR_MAX__ == 0xFFFFFFFF) || (__WCHAR_MAX__ == 0x7FFFFFFF)

#define ZS_TARGET_WCHAR_IS_UTF32

#else // (__WCHAR_MAX__ == 0xFFFFFFFF) || (__WCHAR_MAX__ == 0x7FFFFFFF)

#if (__WCHAR_MAX__ == 0xFFFF) || (__WCHAR_MAX__ == 0x7FFF)

#define ZS_TARGET_WCHAR_IS_UTF16

#else //(__WCHAR_MAX__ == 0xFFFF) || (__WCHAR_MAX__ == 0x7FFF)

#ifdef _WIN32
#define ZS_TARGET_WCHAR_IS_UTF16
#else
#error Unknown WCHAR size. Is this a UTF16 or UTF32 platform?
#endif //_WIN32

#endif // (__WCHAR_MAX__ == 0xFFFF) || (__WCHAR_MAX__ == 0x7FFF)

#endif // (__WCHAR_MAX__ == 0xFFFFFFFF) || (__WCHAR_MAX__ == 0x7FFFFFFF)

  typedef char * STR;
  typedef const char * CSTR;

  typedef TCHAR * TSTR;
  typedef const TCHAR * CTSTR;

  typedef wchar_t * WSTR;
  typedef const wchar_t * CWSTR;

#if __BYTE_ORDER__ == __ORDER_LITTLE_ENDIAN__

#define ZS_TARGET_LITTLE_ENDIAN

#else

#if __BYTE_ORDER__ == __ORDER_BIG_ENDIAN__
#define ZS_TARGET_BIG_ENDIAN
#endif //__BYTE_ORDER__ == __ORDER_BIG_ENDIAN__

#endif //__ORDER_BIG_ENDIAN__

  namespace internal
  {
    ZS_INTERNAL_DECLARE_CLASS_PTR(Settings);
    ZS_INTERNAL_DECLARE_CLASS_PTR(Timer);
    ZS_INTERNAL_DECLARE_CLASS_PTR(MessageQueue);
    ZS_INTERNAL_DECLARE_CLASS_PTR(MessageQueueDispatcher);
    ZS_INTERNAL_DECLARE_CLASS_PTR(MessageQueueThread);
    ZS_INTERNAL_DECLARE_CLASS_PTR(MessageQueueDispatcherForCppWinrt);
    ZS_INTERNAL_DECLARE_CLASS_PTR(MessageQueueDispatcherForWinUWP);
    ZS_INTERNAL_DECLARE_CLASS_PTR(MessageQueueThreadPool);
    ZS_INTERNAL_DECLARE_CLASS_PTR(MessageQueueManager);
  }

}
