function setOrdersChart(xLabels, totalReceived, totalDubbed, totalUploaded, totalShipped)
{
    var data = {
        labels: xLabels,
        datasets: [
            {
                label: "Total Received",
                fill: true,
                lineTension: 0.1,
                backgroundColor: "rgba(192,192,192,0.4)",
                borderColor: "rgba(192,192,192,1)",
                borderCapStyle: 'butt',
                borderDash: [],
                borderDashOffset: 0.0,
                borderJoinStyle: 'miter',
                pointBorderColor: "rgba(192,192,192,1)",
                pointBackgroundColor: "#fff",
                pointBorderWidth: 1,
                pointHoverRadius: 5,
                pointHoverBackgroundColor: "rgba(192,192,192,1)",
                pointHoverBorderColor: "rgba(220,220,220,1)",
                pointHoverBorderWidth: 2,
                pointRadius: 1,
                pointHitRadius: 10,
                data: totalReceived,
                spanGaps: false,
            },
            {
                label: "Total Dubbing",
                fill: false,
                lineTension: 0.1,
                backgroundColor: "rgba(192,75,192,0.4)",
                borderColor: "rgba(192,75,192,1)",
                borderCapStyle: 'butt',
                borderDash: [],
                borderDashOffset: 0.0,
                borderJoinStyle: 'miter',
                pointBorderColor: "rgba(192,75,192,1)",
                pointBackgroundColor: "#fff",
                pointBorderWidth: 1,
                pointHoverRadius: 5,
                pointHoverBackgroundColor: "rgba(192,75,192,1)",
                pointHoverBorderColor: "rgba(220,220,220,1)",
                pointHoverBorderWidth: 2,
                pointRadius: 1,
                pointHitRadius: 10,
                data: totalDubbed,
                spanGaps: false,
            },
            {
                label: "Total Uploaded",
                fill: false,
                lineTension: 0.1,
                backgroundColor: "rgba(192,192,75,0.4)",
                borderColor: "rgba(192,192,75,1)",
                borderCapStyle: 'butt',
                borderDash: [],
                borderDashOffset: 0.0,
                borderJoinStyle: 'miter',
                pointBorderColor: "rgba(192,192,75,1)",
                pointBackgroundColor: "#fff",
                pointBorderWidth: 1,
                pointHoverRadius: 5,
                pointHoverBackgroundColor: "rgba(192,192,75,1)",
                pointHoverBorderColor: "rgba(220,220,220,1)",
                pointHoverBorderWidth: 2,
                pointRadius: 1,
                pointHitRadius: 10,
                data: totalUploaded,
                spanGaps: false,
            },
            {
                label: "Total Shipped",
                fill: false,
                lineTension: 0.1,
                backgroundColor: "rgba(75,192,192,0.4)",
                borderColor: "rgba(75,192,192,1)",
                borderCapStyle: 'butt',
                borderDash: [],
                borderDashOffset: 0.0,
                borderJoinStyle: 'miter',
                pointBorderColor: "rgba(75,192,192,1)",
                pointBackgroundColor: "#fff",
                pointBorderWidth: 1,
                pointHoverRadius: 5,
                pointHoverBackgroundColor: "rgba(75,192,192,1)",
                pointHoverBorderColor: "rgba(220,220,220,1)",
                pointHoverBorderWidth: 2,
                pointRadius: 1,
                pointHitRadius: 10,
                data: totalShipped,
                spanGaps: false,
            }
        ]
    };

    var context = document.getElementById('ordersChart').getContext('2d');
    var stackedLine = new Chart(context, {
        type: 'line',
        data: data,
        options:
            {
                scales:
                    {
                        yAxes: [{
                            stacked: false
                        }]
                    }
            }
    });
}