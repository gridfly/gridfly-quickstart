### GridFly Quickstart

This repository contains examples of using the GridFly API to generate advanced Excel files from HTML.

Implementations are provided for Java, Python, Node and .NET however you can obviously invoke the GridFly API in any language.

All examples use some JSON data as the datasource and use a templating library to generate the HTML. 

Add your client id and secret to the `config.json` file in the root of this repository and then run the example
of your choosing.

You can switch between the GridFly synchronous APIs by setting the mode to either `sync` or `async` in the `config .json`.

Three examples will be run:

### Example 1

A single sheet example demonstrating: 

- subtotalling to 4 levels
- row based conditional formatting highlighting the best month for each store
- row based formulas 

### Example 2

Is a multi-sheet view of the same data used in example 1 demonstrating:

- subtotalling to 4 levels
- row based conditional formatting highlighting the best month for each store
- row based formulas 
- multi-sheet output
- summary sheet creation using the `gf-add-to-summary` and `gf-summary-output` custom attributes and which calculates the overall total 
- range based conditional formatting on the summary sheet to show the best 5 sales months and the best 5 overall.

### Example 3

Is multi sheet view of some more complex data demonstrating: 
- column based conditional formatting  highlighting the top 10% and bottom 10% monthly sales for each product category.
- subtotaling to multiple levels
- row based formulas
- a summary sheet providing a pivoted view of the data generated using GridFly's named range support 
- range based conditional formatting on the summary sheet to highlight the best and worst months overall and for each region.

