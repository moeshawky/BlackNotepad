# Run Tests for BlackNotepad

$roslynPath = "D:\apps\roslyn\tasks\net472"
$csc = "$roslynPath\csc.exe"

# Framework paths
$frameworkPath = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319"
$wpfPath = "$frameworkPath\WPF"
$facadesPath = 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\Facades'

# NuGet packages path
$packagesPath = "$PSScriptRoot\packages"
$testSrcPath = "$PSScriptRoot\BlackNotepad.Test"

# Output DLL
$testDll = "BlackNotepad.Test.dll"

# Test References
$testReferences = @(
    "/reference:`"$frameworkPath\mscorlib.dll`"",
    "/reference:`"$frameworkPath\System.dll`"",
    "/reference:`"$frameworkPath\System.Core.dll`"",
    "/reference:`"$frameworkPath\System.Windows.Forms.dll`"",
    "/reference:`"$wpfPath\WindowsBase.dll`"",
    "/reference:`"$wpfPath\PresentationCore.dll`"",
    "/reference:`"$wpfPath\PresentationFramework.dll`"",
    "/reference:`"$facadesPath\System.Runtime.dll`"",
    "/reference:`"$facadesPath\System.ObjectModel.dll`"",
    "/reference:`"$facadesPath\System.Linq.Expressions.dll`"",
    "/reference:`"$facadesPath\System.Threading.Tasks.dll`"",
    
    # Project Reference
    "/reference:BlackNotepad.exe",

    # Test Packages
    "/reference:`"$packagesPath\MSTest.TestFramework.2.1.2\lib\net45\Microsoft.VisualStudio.TestPlatform.TestFramework.dll`"",
    "/reference:`"$packagesPath\MSTest.TestFramework.2.1.2\lib\net45\Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.dll`"",
    "/reference:`"$packagesPath\Moq.4.15.2\lib\net45\Moq.dll`"",
    "/reference:`"$packagesPath\Castle.Core.4.4.1\lib\net45\Castle.Core.dll`"",
    "/reference:`"$packagesPath\CommonServiceLocator.2.0.5\lib\net47\CommonServiceLocator.dll`"",
    "/reference:`"$packagesPath\MvvmLightLibs.5.4.1.1\lib\net45\GalaSoft.MvvmLight.dll`"",
    "/reference:`"$packagesPath\MvvmLightLibs.5.4.1.1\lib\net45\GalaSoft.MvvmLight.Extras.dll`"",
    "/reference:`"$packagesPath\MvvmLightLibs.5.4.1.1\lib\net45\GalaSoft.MvvmLight.Platform.dll`"",
    "/reference:`"$packagesPath\TestStack.White.0.13.3\lib\net40\TestStack.White.dll`"",
    "/reference:`"$packagesPath\System.Runtime.CompilerServices.Unsafe.5.0.0\lib\net45\System.Runtime.CompilerServices.Unsafe.dll`"",
    "/reference:`"$packagesPath\System.Threading.Tasks.Extensions.4.5.4\lib\net461\System.Threading.Tasks.Extensions.dll`""
)

# Test Source Files
$testFiles = Get-ChildItem "$testSrcPath" -Include *.cs -Recurse | Select-Object -ExpandProperty FullName

Write-Host "Compiling Tests..."
$buildCmd = "$csc /target:library /out:$testDll /platform:anycpu " + ($testReferences -join " ") + " " + ($testFiles -join " ")

try {
    Invoke-Expression $buildCmd
    if (Test-Path $testDll) {
        Write-Host "Tests compiled successfully!"
        
        # Run Tests
        # Use vstest.console.exe if available, otherwise try to find it.
        # Assuming VS is installed or we can find a runner.
        # Let's look for vstest.console.exe
        
        $vstest = "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"
        if (-not (Test-Path $vstest)) {
             $vstest = "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"
        }
        if (-not (Test-Path $vstest)) {
             $vstest = "C:\Program Files (x86)\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"
        }
         if (-not (Test-Path $vstest)) {
             $vstest = "C:\Program Files (x86)\Microsoft Visual Studio\2022\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"
        }
        
        if (Test-Path $vstest) {
            Write-Host "Running tests using $vstest..."
            & $vstest $testDll
        } else {
            Write-Host "vstest.console.exe not found. Please run tests manually."
            # List available tests
            # ...
        }

    }
} catch {
    Write-Host "Test compilation failed: $_"
}
