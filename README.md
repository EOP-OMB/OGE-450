# OGE Form 450: Confidential Financial Disclosure Report
This form is used by executive branch employees who are less senior than public filers to report their financial interests as well as other interests outside the Government. The purpose of this report is to assist employees and their agencies in avoiding conflicts between duties and private financial interests or affiliations.

Description:

The front end server is Windows 2016, this server is running IIS. IIS is configured with two virtual directories associated to Application Pools, WEB and API. Each Application Pool is ran with a distinct Active Directory account, the API App pool is the account with access into the specific List’s and Libraries hosted on SharePoint that contain the data it is handling. No user accounts have direct access into the SharePoint backend.
  
The authentication is provided by ADFS 4.0 running on another Windows 2016 server. ADFS is configured with an application group Relying Party Trust using a GUID. When browsing to the web site to work with the data, the connection is redirected to ADFS to generate a claim. Once the claim token (JWT) is generated for the connection against the Application Group RPT it is used in all subsequent REST calls to the API by including it in the Authentication header.
  
The IIS API validates the JWT token against ADFS for each request made. The token is also used to get the UPN of the user accessing the site. The API then uses SharePoint’s User Profile Service to gather additional information about the user, including roles via CSOM/CAML Queries.
  
The SharePoint farm is running with multiple servers having dedicated roles. There are SharePoint Application servers, SharePoint Web Front End servers, SharePoint Search Servers, and a Microsoft SQL Server, these are all running on Windows Server 2016. All REST/CSOM/CAML Queries are sent to the SharePoint Application Servers.
  
The IIS WEB is a base web site using HTML5, Angular2, TypeScript, and CSS. This site makes REST calls based on the actions PUT/POST/GET to the API. Prior to doing any calls to SharePoint the API ensures the user is authorized to make the call, and that the Authentication Token is still valid. GET requests are pushed to the API, the API then runs a CSOM/CAML query against SharePoint and then responds to the REST call. POST/PUT REST calls are sent to the API then run through business rules on the Posted data to verify nothing was altered, then uses CSOM to save the data to the SharePoint Lists; the data is then returned as a response to the REST call. The WEB then displays the data returned from the REST calls.
  
The Microsoft SQL server is only used by SharePoint and is never directly accessed.
	
When the website is accessed EXTERNALLY all connections are sent through a Web Application Proxy Server sitting on Windows Server 2016, inside the DMZ.

Process Overview

1.	Traffic goes to the website

	a. If internal it goes straight to the website
	
	b. If external it goes through the WAP then to the Website.
  
2.	The website redirects to ADFS to get the bearer token (Auth Token)

3.	ADFS responds to the website with the token if authentication successful

  	a. It uses the Application Group GUID to check the Application Group RPT
	
  	b. It then authenticates the user, if successful it responds with the token.
  
4.	The website then makes a REST call to the API including the bearer token
5.	The API then validates the token against ADFS 
6.	The API will then CSOM/CAML queries to SharePoint

  	a. CSOM/CAML are sent to SharePoint using the AppPool Service Account
  
7.	SharePoint receives the query and responds

  	a. SharePoint Talks to SQL and gets information
	
  	b. SharePoint responds with information gathered
  
8.	API receives the response from SharePoint

  	a. Responses are either filtered in the CAML query or filtered in the API directly
  
9.	API responds to the REST call from the website
10.	Website displays the data

Additional Processes

11.	API Queries LDAP for Accounts
12.	API Sends Information to SharePoint to store in Table
13.	API Queries SharePoint Email List
14.	API Sends Mail not sent to Exchange to be Sent

