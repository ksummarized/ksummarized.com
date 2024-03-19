# Keycloak custom functionalities

This directory contains custom functionalities for Keycloak used in our project.

## How to create a functionality?

Just create a Java class with the functionality in our package `java/com/ksummarized/keycloak/`.

## How to import it into Keycloak?

After creating the functionality there is a need to add our new class into `resources/META-INF/services`. The name of the file will determine where our class will be available. As I created a custom `FormAction` then in order to make it available in form actions in Keycloak I need to add my new class into `FormActionFactory`.

After creating the necessary functionality a new `jar` file needs to be created. It could be achieved by running Maven command:

```cmd
mvn clean package
```

It will create a `target` folder with the new `jar` file. This file needs to be copied into `keycloak/imports/providers/` folder. This folder is linked as a docker volume, so the whole content is inserted to our Keycloak in the Docker.
