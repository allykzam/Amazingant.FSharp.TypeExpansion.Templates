#### 1.2.1 - 2016-12-15
* #5: Fix the tryInnerText FromXml helper returning the wrong type
* #6: Fix indentation and return type in FromXml handling of optional
  collections of nested types

#### 1.2.0 - 2016-12-15
* #4: Add new XPath attribute to the FromXml template with some basic support
  for simple types

#### 1.1.3 - 2016-09-14
* #3: Fix FromXml template trying to set properties as though they were part of
  the main record definition

#### 1.1.2 - 2016-09-09
* #2: Fix some compilation errors in the Lenses template

#### 1.1.1 - 2016-09-09
* #1: Fix some non-valid F# value names being used due to lowercasing of the
  source property names

#### 1.1.0 - 2016-09-08
* Add FromXml template for simple processing of XML documents into record types

#### 1.0.1 - 2016-09-07
* Fix lenses for some types not compiling, e.g. collections, optional types,
  etc.

#### 1.0.0 - 2016-09-06
* Initial release with Lenses template
