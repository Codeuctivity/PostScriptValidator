# PostscriptValidator

This project proofes that your postscript can be parsed by ghostscript. It wrapps call to ghostscript like proposed [here](https://stackoverflow.com/questions/258132/validating-a-postscript-without-trying-to-print-it#2981290) . The nuget package contains ghostscript windows bins and uses preinstalled ghostscript when used in linux.  

```PowerShell
Install-Package PostscriptValidator
```

Sample call:

```csharp
using (var postscriptValidator = new PostScriptValidator.PostScriptValidator())
{
    var result = postscriptValidator.Validate(@"./TestData/valid.ps");
    Assert.True(result);
}
```
