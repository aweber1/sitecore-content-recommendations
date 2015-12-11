# Sitecore Content Recommendation Engine

This is a very basic, very raw sample of how you **might** build a recommendation engine using the Sitecore Path Analyzer API. Path Analyzer is not just for visualization!

## Dependencies
- Sitecore 8.1 (or later).
- There is a TDS project in the solution, but you could restore the items via normal Sitecore serialization also.
- The project was built against Launch Sitecore 8124, but that is not necessarily a hard dependency. The recommendations code can be extracted for use elsewhere.

## Notes
- This is **not** production-ready code. In fact, without some creative workarounds, it likely won't work in a content delivery environment - at the moment. I have some ideas on how to make that happen, so if you're interested let me know.
- The most interesting bits concerning the recommendations generation are located in the `Src/MspUg.Web/Models/Recommendations.cs` file.
- If you are using Launch Sitecore, this project will overwrite the /Views/Shared/Main.cshtml file from the standard Launch Sitecore install. If you already have a Launch Sitecore instance and have made your own changes to main.cshtml, be sure to merge with the file from the web project.
