#### 2.0.2 - 2017-02-24
* Update TypeExpansion paket reference to 1.0.2 to fix the Attributes library
  using the wrong version of FSharp.Core

#### 2.0.1 - 2017-02-24
* Update TypeExpansion paket reference to 1.0.1 to fix test failure

#### 2.0.0 - 2017-02-22
* Update TypeExpansion paket reference to 1.0.0; this is considered a breaking
  change, as it brings along a new version of the FSharp.Core package.

#### 1.5.1 - 2017-02-16
* #17: Fix regression created in 1.3.0. In 1.2.1 and prior, underscores in node
  and attribute names were ignored when searching for a value. This behavior has
  been restored, and additional tests have been added to validate.

#### 1.5.0 - 2017-01-20
* #16: Add new `Validation` attribute and `ValidationResult` types for FromXml
  template; source types can now supply validation methods that will be executed
  after XML processing is complete

#### 1.4.0 - 2017-01-19
* #14: Populate optional fields as `None` if the target node is empty
* #15: Add target field information to exceptions thrown when a `TryParse`
  function indicates failure

#### 1.3.0 - 2017-01-03
* #7: Added support for nested types with the XPath attribute in the FromXml
  template
* #8, 12: Added basic testing script for the FromXml template that includes a
  field for each expected combination of types, fields, and attributes; actual
  expanded results are compared against a reference copy, and parsing of valid
  XML files is tested and validated
* #13: Refactored the FromXml template to be simpler, and to produce moderately
  shorter code
* #11: Added a new RunTests build target to the build script, now required
  before packaging the templates

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
