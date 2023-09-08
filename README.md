# PaymentGateway
Checkout.com .NET take-home challenge  
Completed by: Vladimir Sokolov  
https://github.com/sokoloffvl/PaymentGateway/actions/workflows/dotnet.yml/badge.svg
# Project description  
This solution contains one main executable project `API`, set of different levels abstractions (`Domain`, `Infrastructure`, `MockBank`), and multiple test projects.
Main purpose was to imitate real payment gateway behavior with some limitations and assumptions.
- Api gateway has two main methods: [Create payment](#post-payments) and [Get payment](#get-payments). 
- MockBank is implemented as a separate lib which returns random payment status with a random delay between 0-0.5 seconds.
- Payment processing done asynchronously in the background after request creation. This is accomplished by some naive in-process 
conflating queue implementation.
- PaymentId is assumed to be generated on merchant end
- MockBank calls are retried with increasing wait timeout on retryable payment statuses
- Added primitive api-key authentication (with swagger support)
- There's some basic payment request validation
- As a storage I used IMemoryCache, which is an obvious limitation. However it can be easily switched to some persistent storage 
(any NoSQL would fit nicely)
- Most of the infrastructure elements (logging, metrics, etc.) are either basic or not implemented to pay more attention to the main solution aspects.

For more details see [improvement areas](#areas-of-improvement) or [bonus points](#bonus-points)
# How to run:  
- Use preferred IDE run/debug actions. Requires .net sdk 6 > installed
- Use `docker-compose build` and `docker-compose up` commands. Requires docker installed

# How to use:
- Swagger is available at http://localhost:5001/swagger/index.html
- In order to make http calls from anywhere else see [api usage](#api-description)

# API description
## Authentication
For authentication of your requests use `x-api-key` header. Currently there are three available api-keys:
- `one` for merchant with Id = 1
- `two` for merchant with Id = 2
- `three` for merchant with Id = 3

Ideally these should be provided by auth service or some 3rd party (auth0 or similar) system. Currently everything is hardcoded.
## POST /payments
`POST` http://localhost:5001/payments  

Used for payment request creation. Validation is enabled for most of the fields, i.e. card number should be a valid card number.
### Body example:
``` 
{
  "paymentId": "719cf318-b404-4c1b-9d06-368a55f47419",
  "cardNumber": "378282246310005",
  "cvv": "123",
  "cardOwner": "Card Owner",
  "amount": 100,
  "currency": "USD",
  "validToYear": 29,
  "validToMonth": 11
}
```
### Response codes:
`201` - request been successfully created

`400` - request is invalid

## GET /payments
`GET` http://localhost:5001/payments/{paymentId}  

Used for retrieving current payment info with processing status. If request wasn't processed the `DeclineReason` would be populated.

### Response example:
```
{
  "paymentId": "20b77478-c91e-4f53-882f-610a561c874a",
  "cardNumber": "3782 *** 0005",
  "amount": 100,
  "currency": "USD",
  "status": "Declined",
  "declineReason": "FraudDetected"
}
```
### Response codes:
`200` - request is found

`404` - unknown request id

# Areas of improvement
- There are some flaws in the code due to limited time I had. I tried to cover most of the scenarios by unit and integration tests, but I might've missed something.
My main goal was to give you an idea of how I see this solution implemented, and there might be some bits and pieces that I forgot/missed.
- I was considering CQRS, but currently gateway has only 2 methods and quite simple structure, so I decided to leave CQRS for future consideration
- Logging is quite basic atm, everything is being logged to console. I would consider adding a proper resilience infrastructure around this solution. (metrics, traces, ELK, sentry etc.)
- In-memory storage is not what you want to see in production, so I'd swap it to some persistent storage. In this case I assume some NoSQL db (dynamo db, mongo db, etc.) will fit nicely as there's not that many entity relations. (payment info basically has everything needed)
- Ideally I would split the solution into two separate services. One for gateway itself and one for payment processing. Gateway would do handling of merchant requests and
processing service would do the the payment (call the bank, fraud detection, and everything else). And all parts of this system would be communicating through message bus. Depending on the requirements the second service might be split into more parts.
Happy to discuss that later as there are a lot of details and ideas.
- To imitate asynchronous background payment processing I created a basic in-process queue, which might help if the decision to split service was made.
That part can be substituted for rabbit mq, SNS/SQS, kafka or similar message bus.
- Test coverage is not ideal. Tried to test main usage scenarios, but some things aren't covered yet.
- Currently there's no separate environments (UAT, PROD), most of the values are hardcoded (retries configuration, timeouts, etc.) and secrets storage is missing.
Huge area for improvement.
- Rate limiting
- Add admin/merchant dashboard with preferred UI technology (React, Angular, Blazor) as a separate readonly service
- Event sourcing? Not sure how applicable in this case, but might be useful depending on system requirements (or completely useless :))

# Bonus points
- Authentication
- Polly retries policy
- In-process message queueing
- Central package management (preview feature of nuget)
- Docker support
- Primitive GitHub workflow
- Layered architecture

# Contact details

If you have any questions, please reach out to sokoloffvl@gmail.com


