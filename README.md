# KryptoKid Crypto Currency Portfolio Simulator Web Application

This is the deployment repository for the KryptoKid Web Application. This repository contains the aspnet.core 3.1 sdk and a docker file.


#Features of the application

This application is designed to allow users to sign up and create an account/log in to an existing account. Once an account has been created,
users can buy/sell crypto-currencies based on real-time prices with the virtual balance they have been alotted on sign up. There is no real 
money exhanging hands in this application. This is just a simulation of buying/selling crypto currency to get people exposed to the crypto market 
for free. The user's balance and total value of assets is displayed on the user's account page. The hangfire framework server runs in the background 
and requests the data on cryptocurrencies every 15 minutes. This would be updated up to the minute if there was not a limit on the amount of API calls 
that you can make to the Alpha Vantage API to get crypto-currency data for free. I wanted to keep development cost of this application absolutely free.


# Deployment of this application

To deploy this application, I cloned the this git repository to google cloud console. Once this was complete, I built the application into 
a docker image using the dockerfile. After that, I pushed the image to the google cloud registry. Next, I created a Kubernetes cluster 
using google cloud's Kubernetes Engine. Once the cluster was created. I deployed the application to the Kubernetes cluster and exposed the
Kubernetes' node port to the internet. 


You can visit this application at http://107.178.212.130:31077.


#Construction/Technologies of this application


This appliation was created using the ASP.NET Core Framework version 3.1. I designed this application with a client-server based 
layered architecture. 


Presentation Layer:
This layer consists of javascript, html, and C# taghelpers within MVC View pages. This layer comminucates with the corresponding 
controllers inside the business layer.

(C#, MVC View Pages, Html, JavaScript)


Business Layer:
This layer consists of the ASP.NET Core controllers that process the http get and post requests for the presentation layer. A service 
class from the Persistence layer was injected into this layer to allow for calls to the service class.

(C#, MVC Controllers, Hangfire Framework)


Persistence Layer:
To create this layer I abstracted all of the entity framework data access out of the controller and put it into a "services" class.
This was huge because it keeps the code with the controller very clean and readable. Service method names are very descriptive and 
commented for easy readability. Lastly, and most importantly it keeps the code MUCH more modular and decreases oupling and increases 
cohesion.

(C#, MVC Models, Identity Services, Entity Framework)


Database Layer
This database exists on Google Cloud's Cloud SQL service. It consists of many tables. There are many tables created by the ASP.NET Core Identity
Membership Service. I also created a custom model to add extra properties to the Users table.There are also many tables created by the Hangfire 
framework. I created tables & models for the sales and purchases performed by users within the application. There are also tables and models
for the user's coin portfolio and for the data about the crypto coins such as price, name, etc. Lastly I also created form models for the login and
register actions.

(Google Cloud SQL Server)


#Future Plans
I would like to secure the application with https along with other security features to be determined.
Email Verification
Better Styling
Search bar to search for and view the portfolios of users on the application
Sitewide leaderboard to display the top earners on the application on the home page
Shows your account's earning rank compared to all the other users on the application
More currencies. I would like to someday have to top 50 currencies on the application
Can click on a coin to see its price history and yearly, monthly, daily price charts

