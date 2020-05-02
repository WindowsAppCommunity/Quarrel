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
#include <zsLib/Exception.h>

#ifdef _WIN32
#pragma warning(push)
#pragma warning(disable: 4290)
#endif //_WIN32

#define ZS_JSON_DEFAULT_ATTRIBUTE_PREFIX '$'
#define ZS_JSON_DEFAULT_FORCED_TEXT "#text"

namespace zsLib
{
  namespace XML
  {
    enum ParserWarningTypes
    {
      ParserWarningType_None,

      // XML warnings
      ParserWarningType_MismatchedEndTag,
      ParserWarningType_NoEndBracketFound,
      ParserWarningType_ContentAfterCloseSlashInElement,
      ParserWarningType_ContentAfterCloseElementName,
      ParserWarningType_IllegalAttributeName,
      ParserWarningType_AttributeWithoutValue,
      ParserWarningType_AttributeValueNotFound,
      ParserWarningType_AttributeValueMissingEndQuote,
      ParserWarningType_CDATAMissingEndTag,
      ParserWarningType_NoEndTagFound,
      ParserWarningType_NoEndCommentFound,
      ParserWarningType_NoEndUnknownTagFound,
      ParserWarningType_NoEndDeclarationFound,
      ParserWarningType_NotProperEndDeclaration,
      ParserWarningType_DuplicateAttribute,
      ParserWarningType_ElementsNestedTooDeep,

      // JSON warnings
      ParserWarningType_MustOpenWithObject,
      ParserWarningType_MustCloseRootObject,
      ParserWarningType_MissingObjectClose,
      ParserWarningType_DataFoundAfterFinalObjectClose,
      ParserWarningType_MissingStringQuotes,
      ParserWarningType_InvalidEscapeSequence,
      ParserWarningType_InvalidUnicodeEscapeSequence,
      ParserWarningType_IllegalNumberSequence,
      ParserWarningType_MissingColonBetweenStringAndValue,
      ParserWarningType_AttributePrefixWithoutName,
      ParserWarningType_AttributePrefixAtRoot,
      ParserWarningType_MissingPairString,
      ParserWarningType_IllegalValue,
      ParserWarningType_IllegalArrayAtRoot,
      ParserWarningType_UnexpectedComma,
      ParserWarningType_ParserStuck,
    };

  } // namespace XML
} // namespace zsLib

#include <zsLib/internal/zsLib_XML.h>

namespace zsLib
{
  namespace XML
  {
    struct Exceptions
    {
      ZS_DECLARE_CUSTOM_EXCEPTION(CheckFailed);
    };

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //
    // WalkSink
    //

    class WalkSink
    {
    public:
      // NOTE:  It is safe to call orphan on the current node. If done then
      //        none of children of the orphaned node will be walked.
      virtual bool onDocumentEnter(DocumentPtr inNode) noexcept;
      virtual bool onDocumentExit(DocumentPtr inNode) noexcept;
      virtual bool onElementEnter(ElementPtr inNode) noexcept;
      virtual bool onElementExit(ElementPtr inNode) noexcept;
      virtual bool onAttribute(AttributePtr inNode) noexcept;
      virtual bool onText(TextPtr inNode) noexcept;
      virtual bool onComment(CommentPtr inNode) noexcept;
      virtual bool onDeclarationEnter(DeclarationPtr inNode) noexcept;
      virtual bool onDeclarationExit(DeclarationPtr inNode) noexcept;
      virtual bool onUnknown(UnknownPtr inNode) noexcept;
    };

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //
    // Node
    //

    class Node : public internal::Node
    {
    public:
      struct NodeType
      {
        enum Type
        {
          Document,
          Element,
          Attribute,
          Text,
          Comment,
          Declaration,
          Unknown
        };
      };

      typedef std::list<NodeType::Type> FilterList;

    public:
      ~Node() noexcept;

      DocumentPtr getDocument() const noexcept;

      virtual bool walk(WalkSink &inWalker, const FilterList *inFilterList = NULL) const noexcept;
      virtual bool walk(WalkSink &inWalker, NodeType::Type inType) const noexcept;

      // these methods will return NULL if the related node does not exist
      virtual NodePtr getParent() const noexcept;
      virtual NodePtr getRoot() const noexcept;

      virtual NodePtr getFirstChild() const noexcept;
      virtual NodePtr getLastChild() const noexcept;

      virtual NodePtr getFirstSibling() const noexcept;
      virtual NodePtr getLastSibling() const noexcept;

      virtual NodePtr getPreviousSibling() const noexcept;
      virtual NodePtr getNextSibling() const noexcept;

      virtual ElementPtr getParentElement() const noexcept;
      virtual ElementPtr getRootElement() const noexcept;

      virtual ElementPtr getFirstChildElement() const noexcept;
      virtual ElementPtr getLastChildElement() const noexcept;

      virtual ElementPtr getFirstSiblingElement() const noexcept;
      virtual ElementPtr getLastSiblingElement() const noexcept;

      virtual ElementPtr getPreviousSiblingElement() const noexcept;
      virtual ElementPtr getNextSiblingElement() const noexcept;

      virtual ElementPtr findPreviousSiblingElement(String elementName) const noexcept;
      virtual ElementPtr findNextSiblingElement(String elementName) const noexcept;

      virtual ElementPtr findFirstChildElement(String elementName) const noexcept;
      virtual ElementPtr findLastChildElement(String elementName) const noexcept;

      // checked version of the above methods which throw an exception if they fail to return a valid value instead of returning NULL
      virtual NodePtr getParentChecked() const noexcept(false); // throws Exceptions::CheckFailed
      virtual NodePtr getRootChecked() const noexcept(false); // throws Exceptions::CheckFailed

      virtual NodePtr getFirstChildChecked() const noexcept(false); // throws Exceptions::CheckFailed
      virtual NodePtr getLastChildChecked() const noexcept(false); // throws Exceptions::CheckFailed

      virtual NodePtr getFirstSiblingChecked() const noexcept(false); // throws Exceptions::CheckFailed
      virtual NodePtr getLastSiblingChecked() const noexcept(false); // throws Exceptions::CheckFailed

      virtual NodePtr getPreviousSiblingChecked() const noexcept(false); // throws Exceptions::CheckFailed
      virtual NodePtr getNextSiblingChecked() const noexcept(false); // throws Exceptions::CheckFailed

      virtual ElementPtr getParentElementChecked() const noexcept(false); // throws Exceptions::CheckFailed
      virtual ElementPtr getRootElementChecked() const noexcept(false); // throws Exceptions::CheckFailed

      virtual ElementPtr getFirstChildElementChecked() const noexcept(false); // throws Exceptions::CheckFailed
      virtual ElementPtr getLastChildElementChecked() const noexcept(false); // throws Exceptions::CheckFailed

      virtual ElementPtr getFirstSiblingElementChecked() const noexcept(false); // throws Exceptions::CheckFailed
      virtual ElementPtr getLastSiblingElementChecked() const noexcept(false); // throws Exceptions::CheckFailed

      virtual ElementPtr getPreviousSiblingElementChecked() const noexcept(false); // throws Exceptions::CheckFailed
      virtual ElementPtr getNextSiblingElementChecked() const noexcept(false); // throws Exceptions::CheckFailed

      virtual ElementPtr findPreviousSiblingElementChecked(String elementName) const noexcept(false);
      virtual ElementPtr findNextSiblingElementChecked(String elementName) const noexcept(false);

      virtual ElementPtr findFirstChildElementChecked(String elementName) const noexcept(false);
      virtual ElementPtr findLastChildElementChecked(String elementName) const noexcept(false);

      virtual void orphan() noexcept;                                // this node now is a root element and has no document

      virtual void adoptAsFirstChild(NodePtr inNode) noexcept;       // this node is now adopted as the first child of the current node
      virtual void adoptAsLastChild(NodePtr inNode) noexcept;        // this node is now adopted as the last child of the current node

      virtual void adoptAsPreviousSibling(NodePtr inNode) noexcept;  // this node is now adopted as the previous sibling to the current
      virtual void adoptAsNextSibling(NodePtr inNode) noexcept;      // this node is now adopted as the next sibling from the current

      virtual bool hasChildren() noexcept;                           // does the node have children?
      virtual void removeChildren() noexcept;                        // removes all children

      virtual void clear() noexcept;                                 // remove all contents for the node

      virtual void *getUserData() const noexcept;                    // get private data associated with this node
      virtual void setUserData(void *inData) noexcept;               // set private data associated with this node

      virtual NodeType::Type getNodeType() const noexcept = 0;

      virtual bool isDocument() const noexcept;
      virtual bool isElement() const noexcept;
      virtual bool isAttribute() const noexcept;
      virtual bool isText() const noexcept;
      virtual bool isComment() const noexcept;
      virtual bool isDeclaration() const noexcept;
      virtual bool isUnknown() const noexcept;

      virtual NodePtr         toNode() const noexcept;
      virtual DocumentPtr     toDocument() const noexcept;
      virtual ElementPtr      toElement() const noexcept;
      virtual AttributePtr    toAttribute() const noexcept;
      virtual TextPtr         toText() const noexcept;
      virtual CommentPtr      toComment() const noexcept;
      virtual DeclarationPtr  toDeclaration() const noexcept;
      virtual UnknownPtr      toUnknown() const noexcept;

      virtual NodePtr         toNodeChecked() const noexcept(false);
      virtual DocumentPtr     toDocumentChecked() const noexcept(false);
      virtual ElementPtr      toElementChecked() const noexcept(false);
      virtual AttributePtr    toAttributeChecked() const noexcept(false);
      virtual TextPtr         toTextChecked() const noexcept(false);
      virtual CommentPtr      toCommentChecked() const noexcept(false);
      virtual DeclarationPtr  toDeclarationChecked() const noexcept(false);
      virtual UnknownPtr      toUnknownChecked() const noexcept(false);

      virtual NodePtr clone() const noexcept = 0;                    // creates a clone of this object and clone becomes root object

    protected:
      Node() noexcept;
    };

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //
    // Document
    //

    class Document : public Node,
                     public internal::Document
    {
    public:
      static DocumentPtr create(
                                bool inElementNameIsCaseSensative = true,
                                bool inAttributeNameIsCaseSensative = true
                                ) noexcept;

      static DocumentPtr createFromParsedXML(
                                             const char *inXMLDocument,
                                             bool inElementNameIsCaseSensative = true,
                                             bool inAttributeNameIsCaseSensative = true
                                             ) noexcept;

      static DocumentPtr createFromParsedJSON(
                                              const char *inJSONDocument,
                                              const char *forcedText = ZS_JSON_DEFAULT_FORCED_TEXT,
                                              char attributePrefix = ZS_JSON_DEFAULT_ATTRIBUTE_PREFIX,
                                              bool inElementNameIsCaseSensative = true,
                                              bool inAttributeNameIsCaseSensative = true
                                              ) noexcept;

      static DocumentPtr createFromAutoDetect(
                                              const char *inDocument,
                                              const char *forcedText = ZS_JSON_DEFAULT_FORCED_TEXT,
                                              char attributePrefix = ZS_JSON_DEFAULT_ATTRIBUTE_PREFIX,
                                              bool inElementNameIsCaseSensative = true,
                                              bool inAttributeNameIsCaseSensative = true
                                              ) noexcept;

      // additional methods
      void setElementNameIsCaseSensative(bool inCaseSensative = true) noexcept;
      bool isElementNameIsCaseSensative() const noexcept;

      void setAttributeNameIsCaseSensative(bool inCaseSensative = true) noexcept;
      bool isAttributeNameIsCaseSensative() const noexcept;

      std::unique_ptr<char[]> writeAsXML(size_t *outLengthInChars = NULL) const noexcept;
      std::unique_ptr<char[]> writeAsJSON(size_t *outLengthInChars = NULL) const noexcept;
      std::unique_ptr<char[]> writeAsJSON(
                                          bool prettyPrint,
                                          size_t *outLengthInChars = NULL
                                          ) const noexcept;

      // overrides
      NodePtr clone() const noexcept override;
      void clear() noexcept override;

      NodeType::Type  getNodeType() const noexcept override;
      bool            isDocument() const noexcept override;
      NodePtr         toNode() const noexcept override;
      DocumentPtr     toDocument() const noexcept override;

    public:
      Document(
               const make_private &,
               bool inElementNameIsCaseSensative = true,
               bool inAttributeNameIsCaseSensative = true
               ) noexcept;
    };

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //
    // Element
    //

    class Element : public Node,
                    public internal::Element
    {
    public:
      static ElementPtr create(const char *name = NULL) noexcept;

      void setValue(String inName) noexcept {mName = inName;}

      String getText(
                     bool inCompressWhiteSpace = false,
                     bool inIncludeTextOfChildElements = true
                     ) noexcept;

      String getTextDecoded(
                            bool inCompressWhiteSpace = false,
                            bool inIncludeTextOfChildElements = true
                            ) noexcept;

      AttributePtr findAttribute(String inName) const noexcept;
      String getAttributeValue(String inName) const noexcept;

      bool hasAttributes() const noexcept;
      AttributePtr findAttributeChecked(String inName) const noexcept(false);
      String getAttributeValueChecked(String inName) const noexcept(false);

      bool setAttribute(String inName, String inValue, bool quoted = true) noexcept;  // returns true if replacing existing attribute
      bool setAttribute(AttributePtr inAttribute) noexcept;       // returns true if replacing existing attribute
      bool deleteAttribute(String inName) noexcept;               // remove an existing attribute

      AttributePtr getFirstAttribute() const noexcept;
      AttributePtr getLastAttribute() const noexcept;

      AttributePtr getFirstAttributeChecked() const noexcept(false);
      AttributePtr getLastAttributeChecked() const noexcept(false);

      // overrides
      void adoptAsFirstChild(NodePtr inNode) noexcept override;
      void adoptAsLastChild(NodePtr inNode) noexcept override;

      NodePtr clone() const noexcept override;
      void clear() noexcept override;

      virtual String getValue() const noexcept;                         // returns the element name

      NodeType::Type  getNodeType() const noexcept override;
      bool isElement() const noexcept override;
      NodePtr toNode() const noexcept override;
      ElementPtr toElement() const noexcept override;

    public:
      Element(const make_private &) noexcept;
    };

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //
    // Attribute
    //

    class Attribute : public Node,
                      public internal::Attribute
    {
    public:
      static AttributePtr create() noexcept;

      String getName() const noexcept;          // get the name for the current attribute
      void setName(String inName) noexcept;     // sets the attribute name

      void setValue(String inValue) noexcept;

      void setQuoted(bool inQuoted) noexcept;

      // overrides
      NodePtr getFirstChild() const noexcept override;
      NodePtr getLastChild() const noexcept override;

      NodePtr getFirstSibling() const noexcept override;
      NodePtr getLastSibling() const noexcept override;

      void orphan() noexcept override;                                // this node now is a root element and has no document

      void adoptAsPreviousSibling(NodePtr inNode) noexcept override;  // this node is now adopted as the previous sibling to the current
      void adoptAsNextSibling(NodePtr inNode) noexcept override;      // this node is now adopted as the next sibling from the current

      bool hasChildren() noexcept override;
      void removeChildren() noexcept override;

      NodePtr clone() const noexcept override;
      void clear() noexcept override;

      virtual String getValue() const noexcept;
      virtual String getValueDecoded() const noexcept;

      NodeType::Type  getNodeType() const noexcept override;
      bool            isAttribute() const noexcept override;
      NodePtr         toNode() const noexcept override;
      AttributePtr    toAttribute() const noexcept override;

    public:
      Attribute(const make_private &) noexcept;

    protected:
      void adoptAsFirstChild(NodePtr inNode) noexcept override;       // illegal
      void adoptAsLastChild(NodePtr inNode) noexcept override;        // illegal
    };

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //
    // Text
    //

    class Text : public Node,
                 public internal::Text
    {
    public:
      enum Formats
      {
        Format_EntityEncoded,
        Format_CDATA,
        Format_JSONStringEncoded,
        Format_JSONNumberEncoded,
      };

    public:
      static TextPtr create() noexcept;

      void setValue(const String &inText, Formats format = Format_EntityEncoded) noexcept;    // should be encoded with entities
      void setValueAndEntityEncode(const String &inText) noexcept;
      void setValueAndJSONEncode(const String &inText) noexcept;

      Formats getFormat() const noexcept;

      Formats getOutputFormat() const noexcept;
      void setOutputFormat(Formats format) noexcept;

      // overrides
      bool hasChildren() noexcept override;
      void removeChildren() noexcept override;

      NodePtr clone() const noexcept override;
      void clear() noexcept override;

      virtual String getValue() const noexcept;
      virtual String getValueDecoded() const noexcept;
      virtual String getValueInFormat(
                                      Formats format,
                                      bool normalize = false,
                                      bool encode0xDCharactersInText = false
                                      ) const noexcept;

      NodeType::Type  getNodeType() const noexcept override;
      bool            isText() const noexcept override;
      NodePtr         toNode() const noexcept override;
      TextPtr         toText() const noexcept override;

    public:
      Text(const make_private &) noexcept;

    protected:
      void adoptAsFirstChild(NodePtr inNode) noexcept override;       // illegal
      void adoptAsLastChild(NodePtr inNode) noexcept override;        // illegal
    };

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //
    // Comment
    //

    class Comment : public Node,
                    public internal::Comment
    {
    public:
      static CommentPtr create() noexcept;

      void setValue(String inValue) noexcept {mValue = inValue;}

      // overrides
      bool hasChildren() noexcept override;
      void removeChildren() noexcept override;

      NodePtr clone() const noexcept override;
      void clear() noexcept override;

      virtual String getValue() const noexcept;

      NodeType::Type  getNodeType() const noexcept override;
      bool            isComment() const noexcept override;
      NodePtr         toNode() const noexcept override;
      CommentPtr      toComment() const noexcept override;

    public:
      Comment(const make_private &) noexcept;

    protected:
      void adoptAsFirstChild(NodePtr inNode) noexcept override;       // illegal
      void adoptAsLastChild(NodePtr inNode) noexcept override;        // illegal
    };

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //
    // Declaration
    //

    class Declaration : public Node,
                        public internal::Declaration
    {
    public:
      static DeclarationPtr create() noexcept;

      AttributePtr findAttribute(String inName) const noexcept;
      String getAttributeValue(String inName) const noexcept;

      AttributePtr findAttributeChecked(String inName) const noexcept(false);
      String getAttributeValueChecked(String inName) const noexcept(false);

      bool setAttribute(String inName, String inValue) noexcept;  // returns true if replacing existing attribute
      bool setAttribute(AttributePtr inAttribute) noexcept;       // returns true if replacing existing attribute
      bool deleteAttribute(String inName) noexcept;

      AttributePtr getFirstAttribute() const noexcept;
      AttributePtr getLastAttribute() const noexcept;

      AttributePtr getFirstAttributeChecked() const noexcept(false);
      AttributePtr getLastAttributeChecked() const noexcept(false);

      // overrides
      void adoptAsFirstChild(NodePtr inNode) noexcept override;    // can only add attribute children
      void adoptAsLastChild(NodePtr inNode) noexcept override;     // can only add attribute children

      NodePtr clone() const noexcept override;
      void clear() noexcept override;

      NodeType::Type  getNodeType() const noexcept override;
      bool            isDeclaration() const noexcept override;
      NodePtr         toNode() const noexcept override;
      DeclarationPtr  toDeclaration() const noexcept override;

    public:
      Declaration(const make_private &) noexcept;
    };

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //
    // Unknown
    //

    class Unknown : public Node,
                    public internal::Unknown
    {
    public:
      static UnknownPtr create() noexcept;

      void setValue(String inValue) noexcept  {mValue = inValue;}

      // overrides
      bool hasChildren() noexcept override;
      void removeChildren() noexcept override;

      NodePtr clone() const noexcept override;
      void clear() noexcept override;

      virtual String getValue() const noexcept;

      NodeType::Type  getNodeType() const noexcept override;
      bool            isUnknown() const noexcept override;
      NodePtr         toNode() const noexcept override;
      UnknownPtr      toUnknown() const noexcept override;

    public:
      Unknown(const make_private &) noexcept;

    protected:
      void adoptAsFirstChild(NodePtr inNode) noexcept override;       // illegal
      void adoptAsLastChild(NodePtr inNode) noexcept override;        // illegal
    };

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //
    // ParserPos
    //

    class ParserPos : public internal::ParserPos
    {
    public:
      ULONG mRow;
      ULONG mColumn;
      const char *mPos;

    public:
      ParserPos() noexcept;
      ParserPos(const ParserPos &) noexcept;
      ParserPos(
                ParserPtr inParser,
                DocumentPtr inDocument
                ) noexcept;

      bool isSOF() const noexcept;  // SOF = start of file
      bool isEOF() const noexcept;  // EOF = end of file

      void setSOF() noexcept; // force the parse pos to the start of the file
      void setEOF() noexcept; // force the parse pos to be at the end of the file

      void setParser(ParserPtr inParser) noexcept;
      ParserPtr getParser() const noexcept;

      void setDocument(DocumentPtr inDocument) noexcept;
      DocumentPtr getDocument() const noexcept;

      ParserPos operator++() const noexcept;
      ParserPos &operator++() noexcept;

      ParserPos operator--() const noexcept;
      ParserPos &operator--() noexcept;

      size_t operator-(const ParserPos &inPos) const noexcept;

      ParserPos &operator+=(size_t inDistance) noexcept;
      ParserPos &operator-=(size_t inDistance) noexcept;

      bool operator==(const ParserPos &inPos) noexcept;
      bool operator!=(const ParserPos &inPos) noexcept;

      char operator*() const noexcept;

      operator CSTR() const noexcept;

      bool isString(CSTR inString, bool inCaseSensative = true) const noexcept;

    protected:
      friend class Parser;
      friend class Document;

      ParserPos(Parser &, Document &) noexcept;
    };

    ParserPos operator+(const ParserPos &inPos, size_t inDistance) noexcept;
    ParserPos operator-(const ParserPos &inPos, size_t inDistance) noexcept;
    ParserPos operator+(const ParserPos &inPos, int inDistance) noexcept;
    ParserPos operator-(const ParserPos &inPos, int inDistance) noexcept;


    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //
    // ParserWarning
    //

    class ParserWarning
    {
    public:
      struct ParserInfo
      {
        ParserPos mPos;      // the const char * location is only valid for the lifetime of the buffer passed into the parser
        String mXMLSnip;
      };
      typedef std::list<ParserInfo> ParserStack;

    public:
      ParserWarningTypes mWarningType;
      ParserStack mStack;

      ParserWarning(const ParserWarning &source) noexcept;
      ~ParserWarning() noexcept;

      String getAsString(bool inIncludeEntireStack = true) const noexcept;

    protected:
      friend class XML::internal::Parser;
      ParserWarning(
                    ParserWarningTypes inWarningType,
                    const XML::internal::Parser::ParserStack &inStack
                    ) noexcept;
    };

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //
    // Parser
    //

    class Parser : public internal::Parser
    {
    public:
      typedef internal::Parser::NoChildrenElementList NoChildrenElementList;

    public:
      static ParserPtr createXMLParser() noexcept;
      static ParserPtr createJSONParser(
                                        const char *forcedText = ZS_JSON_DEFAULT_FORCED_TEXT,
                                        char attributePrefix = ZS_JSON_DEFAULT_ATTRIBUTE_PREFIX
                                        ) noexcept;

      static ParserPtr createAutoDetectParser(
                                              const char *jsonForcedTextElement = ZS_JSON_DEFAULT_FORCED_TEXT,
                                              char jsonAttributePrefix = ZS_JSON_DEFAULT_ATTRIBUTE_PREFIX
                                              ) noexcept;

      virtual DocumentPtr parse(
                                const char *inDocument,
                                bool inElementNameIsCaseSensative = true,
                                bool inAttributeNameIsCaseSensative = true
                                ) noexcept;  // must be terminated with a NUL character

      void clearWarnings() noexcept;
      const Warnings &getWarnings() const noexcept;
      void enableWarnings(bool inEnableWarnings) noexcept {mEnableWarnings = inEnableWarnings;}
      bool areWarningsEnabled() noexcept {return mEnableWarnings;}

      ULONG getTabSize() const noexcept;
      void setTabSize(ULONG inTabSize) noexcept;

      void setNoChildrenElements(const NoChildrenElementList &noChildrenElementList) noexcept;

      ParserPtr toParser() const noexcept {return mThis.lock();}

      // helper methods
      static String convertFromEntities(const String &inString) noexcept;

      static String makeTextEntitySafe(const String &inString, bool entityEncode0xD = false) noexcept;
      static String makeAttributeEntitySafe(const String &inString, char willUseSurroundingQuotes = 0) noexcept;

      static String convertFromJSONEncoding(const String &inString) noexcept;
      static String convertToJSONEncoding(const String &inString) noexcept;

    public:
      Parser(const make_private &) noexcept;
    };

    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
    //
    // Generator
    //

    class Generator : public internal::Generator
    {
    public:
      enum XMLWriteFlags
      {
        XMLWriteFlag_None =                   0x00000000,
        XMLWriteFlag_ForceElementEndTag =     0x00000001,
        XMLWriteFlag_NormalizeCDATA =         0x00000002,
        XMLWriteFlag_EntityEncode0xDInText =  0x00000004, // requires WriteFlag_NormalizeCDATA is set
        XMLWriteFlag_NormizeAttributeValue =  0x00000008,
      };

      enum JSONWriteFlags
      {
        JSONWriteFlag_None =                   0x00000000,
        JSONWriteFlag_PrettyPrint =            0x00000010,
      };

    public:
      static GeneratorPtr createXMLGenerator(XMLWriteFlags writeFlags = XMLWriteFlag_None) noexcept;
      static GeneratorPtr createJSONGenerator(
                                              const char *forcedText = ZS_JSON_DEFAULT_FORCED_TEXT,
                                              char attributePrefix = ZS_JSON_DEFAULT_ATTRIBUTE_PREFIX
                                              ) noexcept;
      static GeneratorPtr createJSONGenerator(
                                              JSONWriteFlags writeFlags,
                                              const char *forcedText = ZS_JSON_DEFAULT_FORCED_TEXT,
                                              char attributePrefix = ZS_JSON_DEFAULT_ATTRIBUTE_PREFIX
                                              ) noexcept;

      virtual size_t getOutputSize(const NodePtr &onlyThisNode) const noexcept;
      virtual std::unique_ptr<char[]> write(const NodePtr &onlyThisNode, size_t *outLengthInChars = NULL) const noexcept;

      virtual GeneratorPtr toGenerator() const noexcept;

      virtual XMLWriteFlags getXMLWriteFlags() const noexcept;
      virtual JSONWriteFlags getJSONWriteFlags() const noexcept;

    public:
      Generator(
                const make_private &,
                UINT writeFlags
                ) noexcept;
    };

  } // namespace XML

} // namespace zsLib

#ifdef _WIN32
#pragma warning(pop)
#endif // _WIN32
