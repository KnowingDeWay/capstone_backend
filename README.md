# capstone_backend

Back End Setup:

1.	Open Visual Studio 2019

2.	Click “Open a project or solution” (if this is your first time)
 
3.	Open this solution file in the repo folder:
 
4.	Your screen should look like the following when the solution file is opened
 
5.	Open the Package Manager Console by going to View > Other Windows > Package Manager Console
 
6.	Typical .NET/C# projects require the use of NuGet packages which can be easily installed via a special menu, however, to just install all the existing NuGet package dependencies, type the command “dotnet restore” into the Package Manager Console. This must be done because NuGet packages and the code files within are not and should not be committed into GitHub or into any source control repo in general.
 
Optional: To install further NuGet packages if you wish to do so, you can access the GUI for this via the following:
First, right click on the web project
 
Then, click “Manage NuGet Packages” and the following screen should appear:
 
If you want to install a new NuGet packages from the net, click the “Browse” tab and look up your desired package. The packages in the “Installed” tab shows the packages that have already been installed

7.	To run the application, first make sure “IIS Express” is selected and then click on the button with the green play button
 
8.	The following should appear on your default browser after the project has been executed
 
The above Swagger window details all the endpoints for each controller in the back-end web API. To use this API, click on the Login User endpoint and enter valid login credentials, then click “Execute”
 
After that, copy the content in the “reponseToken” message, without the quotation marks, open the Authorize window at the top of the page
 
In the resulting window, type, the word “Bearer” with a space and then paste the retrieved token and then click “Authorize”.
 
After doing all the above, you will have access to all the API endpoints in Swagger if you wish to use those endpoints as all endpoints are protected and require a valid encoded JWT token to access lest your requests be marked as “Unauthorized” (HTTP code 401 or 403) by the system.
When you are done, you can logout using the following endpoint,
 
If you are logged in, clicking “Execute” will log you out and will destroy your JWT token and end your session entirely.
 
A HTTP code of 200 indicates that a request was “Successful” and there were no issues involved in handling that request.

9.	To stop the application or to stop debugging, click on the “Stop” button which is a button with a red square. This, however, closes the Swagger window automatically when the server stops running
