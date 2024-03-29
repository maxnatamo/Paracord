# Writing test suites

We recommend creating test-suites for all newly implemented features, within reason, as it will prevent accidental breaking changes.

All tests are executed on pull requests and on all commits to `main`.

## Structuring

When creating new test classes, they should conform to some guidelines, to avoid confusion. Generally, test files should be structured as such:
 Test type          | File name 
--------------------|-------
 Unit tests         | `src/{PROJECT}/test/{FOLDERPATH}/{CLASSNAME}/{METHODNAME}Tests.cs`
 Integration tests  | `src/{PROJECT}/test/{FOLDERPATH}/{CLASSNAME}Tests.cs`

- `PROJECT`: The parent project where the tested class resides, such as `Core`, `Shared`, etc.
- `FOLDERPATH`: The path to the tested class in the source project.
- `CLASSNAME`: The name of the tested class.
- `METHODNAME`: The method being tested.

For example, with the following source tree:

```
Paracord.Shared/
  Security/
    X509/
      X509CertificateBuilder.cs
  ...
```

When unit-testing the `Build` method in `X509CertificateBuilder`, the test file should be named like this:
```
src/Shared/test/Security/X509/X509CertificateBuilder/BuildTests.cs
```

## Namespaces

Like file names, the namespace of the test file is equally important. It should be specific enough to not have
collisions with other tests; or even the type being tested.

Currently, the project uses a namespace convention, like so:
```
Paracord.{PROJECT}.Tests.{FOLDERPATH}.{CLASSNAME}Tests
```

## Methods

The test method name should indicate the expected result and a description of the input, as such:

```
{METHODNAME}_Returns{EXPECTED_RESULT}_Given{INPUT}
```

- `EXPECTED_RESULT`: The expected result of the method, such as `ReturnsTrue`, `ThrowsUnexpectedTokenException`, etc.
- `INPUT`: A simple description of the input, such as `EmptyString`, `IntegerAboveMax`, `NonPrintableAscii`, etc.