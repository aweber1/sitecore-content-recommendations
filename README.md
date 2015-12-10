# Sitecore Content Recommendation Engine

This is a very basic, very raw sample of how you could build a recommendation engine using the Sitecore Path Analyzer API. Path Analyzer is not just for visualization!

## Dependencies
- Launch Sitecore 8124 (or later).
- There is a TDS project in the solution, but you could restore the items via normal Sitecore serialization also.

## Notes
- This project will overwrite the /Views/Shared/Main.cshtml file from the standard Launch Sitecore install. If you already have a Launch Sitecore instance and have made your own changes to main.cshtml, be sure to merge with the file from the web project.
