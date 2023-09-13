# Lucy's Lemonade Stand

This repository is meant to show how to write all sorts of different tests.

## Requirements for an API:

* Pitcher (singleton) - available cups
* Cash Box (singleton) (simple $ amount)
* Price List (singleton) ($ per cup, $ per refill of 8 cups from mom)
* Buy (cups amount, cash)
* Purchase Processor (Order, $ given) => 
    * error if Order.cups * \$ per cups > $ given
    * error if Order.cups > Pitcher's available cups
    * Pitcher's available -= order.cups
    * register.cash += order.cups * $ per cup
    * return cups and change (\$ given - (order.cups * $ per cup))
* When pitcher is empty or close to empty, ask mom for more (external API call)
    * Mom might randomly say no (429 Too Many Requests) some percent of the time because she's tired.