::==== ~Root ====
nuget push "Dawnx/bin/Release/Dawnx.1.9.34.nupkg" -source nuget.org
nuget push "Dawnx.Tools/bin/Release/Dawnx.Tools.1.9.34.nupkg" -source nuget.org
nuget push "Dawnx.Diagnostics/bin/Release/Dawnx.Diagnostics.1.9.34.nupkg" -source nuget.org
nuget push "DawnxLite/bin/Release/DawnxLite.1.9.34.nupkg" -source nuget.org

::==== ~AspNetCore ====
nuget push "~Library/Dawnx.AspNetCore/bin/Release/Dawnx.AspNetCore.1.9.34.nupkg" -source nuget.org
nuget push "~Library/Dawnx.AspNetCore.IdentityUtility/bin/Release/Dawnx.AspNetCore.IdentityUtility.1.9.34.nupkg" -source nuget.org
nuget push "~Library/Dawnx.AspNetCore.LiveAccountUtility/bin/Release/Dawnx.AspNetCore.LiveAccountUtility.1.9.34.nupkg" -source nuget.org

::==== ~Dawnx.Library ===
nuget push "~Library/Dawnx.LuaEngine/bin/Release/Dawnx.LuaEngine.1.9.34.nupkg" -source nuget.org
nuget push "~Library/Dawnx.Net/bin/Release/Dawnx.Net.1.9.34.nupkg" -source nuget.org
nuget push "~Library/Dawnx.NPOI/bin/Release/Dawnx.NPOI.1.9.34.nupkg" -source nuget.org
nuget push "~Library/Dawnx.Security/bin/Release/Dawnx.Security.1.9.34.nupkg" -source nuget.org
nuget push "~Library/Dawnx.Xml/bin/Release/Dawnx.Xml.1.9.34.nupkg" -source nuget.org

::==== ~Dawnx.Chinese ===
nuget push "~Localization/Dawnx.Chinese/bin/Release/Dawnx.Chinese.1.9.34.nupkg" -source nuget.org

pause
