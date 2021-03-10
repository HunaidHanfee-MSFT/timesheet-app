// <copyright file="project-utilization.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

export default interface IProjectUtilization {
    id: string;
    title: string;
    billableHours: number;
    nonBillableHours: number;
    notUtilizedHours: number;
    projectStartDate: Date;
    projectEndDate: Date;
}