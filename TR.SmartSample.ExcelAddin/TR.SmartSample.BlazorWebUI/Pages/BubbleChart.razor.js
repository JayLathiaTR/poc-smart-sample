console.log("Loading BubbleChart.razor.js");

export async function createTable(serviceData) {
    await Excel.run(async (context) => {
        context.workbook.worksheets.getItemOrNullObject("Sample").delete();
        const sheet = context.workbook.worksheets.add("Sample");

        const inventoryTable = sheet.tables.add("A1:D1", true);
        inventoryTable.name = "Sales";
        inventoryTable.getHeaderRowRange().values = [["Product", "Inventory", "Price", "Current Market Share"]];

        const rows = serviceData.map(item => [item.product, item.inventory, item.price, item.currentMarketShare]);
        inventoryTable.rows.add(null, rows);

        sheet.getUsedRange().format.autofitColumns();
        sheet.getUsedRange().format.autofitRows();

        sheet.activate();
        await context.sync();
    });
}

export async function createBubbleChart() {
    await Excel.run(async (context) => {
        /*
          The table has the following columns and data.
          Product, Inventory, Price, Current Market Share
          Calamansi, 2000, $2.45, 10%
          ...
    
          Each bubble represents a single row.
        */

        // Get the worksheet and table data.
        const sheet = context.workbook.worksheets.getItem("Sample");
        const table = sheet.tables.getItem("Sales");
        const dataRange = table.getDataBodyRange();

        // Get the table data without the row names.
        const valueRange = dataRange.getOffsetRange(0, 1).getResizedRange(0, -1);

        // Create the chart.
        const bubbleChart = sheet.charts.add(Excel.ChartType.bubble, valueRange);
        bubbleChart.name = "Product Chart";

        // Remove the default series, since we want a unique series for each row.
        bubbleChart.series.getItemAt(0).delete();

        // Load the data necessary to make a chart series.
        dataRange.load(["rowCount", "values"]);
        await context.sync();

        // For each row, create a chart series (a bubble).
        for (let i = 0; i < dataRange.rowCount; i++) {
            const newSeries = bubbleChart.series.add(dataRange.values[i][0], i);
            newSeries.setXAxisValues(dataRange.getCell(i, 1));
            newSeries.setValues(dataRange.getCell(i, 2));
            newSeries.setBubbleSizes(dataRange.getCell(i, 3));

            // Show the product name and market share percentage.
            newSeries.dataLabels.showSeriesName = true;
            newSeries.dataLabels.showBubbleSize = true;
            newSeries.dataLabels.showValue = false;
        }

        await context.sync();
    });
}