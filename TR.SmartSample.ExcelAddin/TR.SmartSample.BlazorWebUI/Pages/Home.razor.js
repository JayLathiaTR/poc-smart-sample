console.log("Loading Home.razor.js");

export function helloButton(dynamicValue) {

    console.log("We are now entering function: helloButton");

    return Excel.run(context => {
        // Insert dynamic text into cell A1.
        context.workbook.worksheets.getActiveWorksheet().getRange("A1").values = [[dynamicValue]];

        // sync the context to run the previous API call, and return.
        return context.sync();
    });
}