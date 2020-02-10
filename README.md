# PostscriptValidator

[![Build status](https://ci.appveyor.com/api/projects/status/idve16xnoe1sgphv/branch/master?svg=true)](https://ci.appveyor.com/project/stesee/postscriptvalidator/branch/master)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/c2e7bef317364aecbf9c0675a808c9e2)](https://www.codacy.com/manual/stesee/PostScriptValidator?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Codeuctivity/PostScriptValidator&amp;utm_campaign=Badge_Grade)
[![Nuget](https://img.shields.io/nuget/v/PostscriptValidator.svg)](https://www.nuget.org/packages/PostscriptValidator/)

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

# Dependencies

Windows: 
* none 

Ubuntu: 
```bash
suod apt install ghostscript
```
