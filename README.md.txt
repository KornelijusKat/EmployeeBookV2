Employee registration application built on .Net core webapp using .net 6.0. 
In this application a user makes an account(user) which he receives an user role for authentification purposes, cookie authorization is used to authenticate/authorize.
Once logged in the user can see cards with information and pictures of every profile created. The user can create a profile but only one with a picture. The picture will downsized and turned into a byte[] and then fit into a MySql database.
An admin can see extra buttons like edit and delete on every profile, can see options to see all users and do the same with them.
All password are hashed and stored in the database.
More features to be added as for now it's mostly just a registration application.
 