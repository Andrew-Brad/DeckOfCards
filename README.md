# What is this project?

This is a living project that tries to demonstrate the the principles of Clean Architure using Command-Query Responsibility Segregation, domain events, and in-process messaging.

At a high level, it's an Api written in Asp.Net Core that allows us to interact with playing cards.

![alt text](https://get.pxhere.com/photo/deck-bicycle-pattern-ace-card-brand-illustration-design-calligraphy-games-playing-card-magic-cards-1068201.jpg "Logo Title Text 1")

I try to structure the project such that it involves a commonly understood domain (everyone understands how to interact with a deck of cards), but also employs domain concepts that are less trivial than 'todo lists' and the typical Northwind relation database examples.

Currently, the project is being migrated from some old code, and is currently in transition with a different database backend, and moving to Azure Pipelines CI with Docker.

I maintain this as a living project, and it takes dependencies on top-notch libraries that I've been using in production software for years.