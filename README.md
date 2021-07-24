# anv-face-rec-demo

How To Run:
1. Clone the repo
2. cd to the /src folder
3. run `docker-compose up (-d)` (-d it optional if you prefer detached)
4. The client is actually a web service with a Swagger UI to test the service accessible at: http://localhost:3000/swagger/index.html
5. The server also has a Swagger UI at: http://localhost:5000/swagger/index.html
  Note: The FindPerson API is a GET, but uses a body to pass the features. Swagger doesn't support GET with body so test can be done in Postman.
6. An adminer is also available to access the DB at: http://localhost:8080/. 
  Connection details are in the docker-compose postgres section.
