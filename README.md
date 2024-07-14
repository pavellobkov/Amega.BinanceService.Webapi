					Test task description


	On of the main task “single connection to the data provider” I implemented in class “BinanceDataProvider”, which made according to the pattern “Singleton” with private constructor. When object of this class creates in method “Subscribe” we begin to read prices from “stream.binance.com” and in this class we have three properties for every currency we needs. So from this  properties we can get current prices for currencies.

	“provide REST API and WebSocket”. I implemented this point in Middleware of this project. So you can connect o the service both via web sockets and HTTP requests. 

	“Efficiently manage 1,000+ WebSocket subscribers”. I implemented endpoint in controller and sending response from web sockets as asynchronous methods. So this point will depend, among other things, from server with this service. Also I set “instants” in data provider class as “volatile” and useful field might be modified by multiple threads that are executing at the same time.

	Of course, this project can be improved, logging can be expanded, caching can be added through Reddis, or through a Hashtable, but I only fulfilled the basic requirements in order to meet the deadline
