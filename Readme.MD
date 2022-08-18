# Security Question Console App
This is a simple console application with a simple data store: LiteDb https://www.litedb.org/
Data is stored with a fast and ready hashing mechanism for some basic security, but nothing you would normally use for credential information.

## Slacker Plumbing
Slacker.Repository is a trimmed down updated version of a private personal nuget package currently housed in a personal Azure Devops Orginization. there is a Mongo implementation with other NoSQL stores to follow.  There are other libraries for Stored Procedures, and Bus abstractions.


## TODO
1. Add logging with Serilog and structured logging
2. Refactor so that Flow methods are housed in classes implementing IFlow and add FlowManagerService so that flow logic can be unit tested.
3. Unit tests with AutoFixture and auto proxy mocking
4. polish documentation