// Type definitions for Angular Material 1.0.0-rc5+ (angular.material module)
// Project: https://github.com/angular/material
// Definitions by: Matt Traynham <https://github.com/mtraynham>
// Definitions: https://github.com/borisyankov/DefinitelyTyped

/// <reference path="../chart.js/index.d.ts" />

interface KsLineChartData extends ChartData {
    labels?: string[];
    datasets?: KsLineChartDataSets[];
}

interface KsPieChartData extends ChartData {
    labels?: string[];
    datasets?: KsPieChartDataSets[];
}

interface KsLineChartDataSets {
    backgroundColor?: ChartColor;
    borderWidth?: number;
    borderColor?: ChartColor;
    borderCapStyle?: string;
    borderDash?: number[];
    borderDashOffset?: number;
    borderJoinStyle?: string;
    data?: number[] | ChartPoint[];
    fill?: boolean;
    label?: string;
    lineTension?: number;
    pointBorderColor?: ChartColor | ChartColor[];
    pointBackgroundColor?: ChartColor | ChartColor[];
    pointBorderWidth?: number | number[];
    pointRadius?: number | number[];
    pointHoverRadius?: number | number[];
    pointHitRadius?: number | number[];
    pointHoverBackgroundColor?: ChartColor | ChartColor[];
    pointHoverBorderColor?: ChartColor | ChartColor[];
    pointHoverBorderWidth?: number | number[];
    pointStyle?: string | string[] | HTMLImageElement | HTMLImageElement[];
    xAxisID?: string;
    yAxisID?: string;
}

interface KsPieChartDataSets {
    backgroundColor?: ChartColor[];
    borderWidth?: number;
    borderColor?: ChartColor;
    borderCapStyle?: string;
    borderDash?: number[];
    borderDashOffset?: number;
    borderJoinStyle?: string;
    data?: number[] | ChartPoint[];
    fill?: boolean;
    label?: string;
    lineTension?: number;
    pointBorderColor?: ChartColor | ChartColor[];
    pointBackgroundColor?: ChartColor | ChartColor[];
    pointBorderWidth?: number | number[];
    pointRadius?: number | number[];
    pointHoverRadius?: number | number[];
    pointHitRadius?: number | number[];
    pointHoverBackgroundColor?: ChartColor | ChartColor[];
    pointHoverBorderColor?: ChartColor | ChartColor[];
    pointHoverBorderWidth?: number | number[];
    pointStyle?: string | string[] | HTMLImageElement | HTMLImageElement[];
    xAxisID?: string;
    yAxisID?: string;
}