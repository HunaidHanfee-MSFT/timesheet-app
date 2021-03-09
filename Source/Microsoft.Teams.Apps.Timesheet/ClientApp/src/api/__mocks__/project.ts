// <copyright file="project.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import { StatusCodes } from "http-status-codes";
import IProjectMember from "../../models/project-member";
import IProjectMemberOverview from "../../models/project-member-overview";
import IProjectTaskOverview from "../../models/project-task-overview";
import IProjectUtilization from "../../models/project-utilization";
import { Guid } from "guid-typescript";
import { IDashboardProject } from "../../models/dashboard/dashboard-project";

const projectUtilization: IProjectUtilization = {
    billableUtilizedHours: 30,
    nonBillableUtilizedHours: 30,
    billableUnderutilizedHours: 20,
    nonBillableUnderutilizedHours: 20,
    totalHours: 100,
    id: "1212",
    title: "Test",
    projectEndDate: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate() + 1),
    projectStartDate: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate() + 2)
};

const memberOverview: Array<IProjectMemberOverview> = [
    { id: "34344", isBillable: true, isRemoved: false, isSelected: false, projectId: "1212", totalHours: 50, userId: "1212", userName: "demo 1" },
    { id: "34345", isBillable: true, isRemoved: false, isSelected: false, projectId: "1212", totalHours: 50, userId: "1213", userName: "demo 1" }
];

const projectTaskOverview: Array<IProjectTaskOverview> = [
    {
        id: Guid.createEmpty().toString(), isRemoved: false, isSelected: false, projectId: "1212", title: "test", totalHours: 44, startDate: new Date(), endDate: new Date()
    }
];

const dashboardProjects: Array<IDashboardProject> = [
    {
        id: Guid.createEmpty(), title: "Project X", totalHours: 5, utilizedHours: 10
    }
];

/**
 * Get approved and active project details for dashboard between date range.
 * @param startDate The start date of the date range.
 * @param endDate The end date of the date range.
 * @param  {VoidFunction} handleTokenAccessFailure Call back to handle token access failure and redirect to sign-in page.
 */
export const getDashboardProjectsAsync = async (
    startDate: Date,
    endDate: Date,
    handleTokenAccessFailure: (error: string) => void) => {
    return Promise.resolve({
        data: dashboardProjects,
        status: StatusCodes.OK
    });
};

/**
 * Get project utilization details between date range.
 * @param projectId The project Id of which project details to get.
 * @param startDate The start date of the date range.
 * @param endDate The end date of the date range.
 */
export const getProjectUtilizationAsync = async (
    projectId: string,
    startDate: Date,
    endDate: Date) => {
    return Promise.resolve({
        data: projectUtilization,
        status: StatusCodes.OK
    });
};

/**
 * Upload image photo
 * @param formData Form data containing selected image
 * @param teamId The LnD team ID
 */
export const addMembersAsync = async (members: Array<IProjectMember>) => {
    return Promise.resolve({
        data: true,
        status: StatusCodes.OK
    });
};

/**
 * The API which handles request to update members.
 * @param projectId The Id of the project in which members need to be updated.
 * @param members The details of members to be updated.
 * @param  {VoidFunction} handleTokenAccessFailure Call back to handle token access failure and redirect to sign-in page.
 */
export const deleteMembersAsync = async (
    projectId: string,
    members: Array<IProjectMemberOverview>,
    handleTokenAccessFailure: (error: string) => void) => {
    return Promise.resolve({
        data: true,
        status: StatusCodes.NO_CONTENT
    })
};

/**
 * The API which handles request to update task.
 * @param projectId The Id of the project in which tasks need to be updated.
 * @param tasks The details of tasks to be updated.
 * @param  {VoidFunction} handleTokenAccessFailure Call back to handle token access failure and redirect to sign-in page.
 */
export const deleteTasksAsync = async (projectId: string, taskIds: Array<string>, handleTokenAccessFailure: (error: string) => void) => {
    return Promise.resolve({
        data: true,
        status: StatusCodes.NO_CONTENT
    });
};

/**
 * Save event as draft
 * @param event Event details to be saved as draft
 * @param teamId The LnD team ID
 */
export const updateMembersAsync = async (members: Array<IProjectMemberOverview>) => {
    return Promise.resolve({
        data: true,
        status: StatusCodes.OK
    });
};

/**
 * Update draft event
 * @param event Event details to be updated as draft
 * @param teamId The LnD team ID
 */
export const getProjectMembersOverviewAsync = async (projectId: string,
    startDate: Date,
    endDate: Date) => {
    return Promise.resolve({
        data: memberOverview,
        status: StatusCodes.OK
    });
};

/**
 * The API which handles request to create new tasks.
 * @param tasks The details of tasks to be created.
 */
export const createTasksAsync = async (
    tasks: Array<IProjectTaskOverview>) => {
    for (var i = 0; i < tasks.length; i++) {
        tasks[i].id = Guid.createEmpty().toString();
        projectTaskOverview.push(tasks[i]);
    }
    return Promise.resolve({
        data: true,
        status: StatusCodes.OK
    });
};

/**
 * The API which handles request to update task.
 * @param tasks The details of tasks to be updated.
 */
export const updateTasksAsync = async (
    tasks: Array<IProjectTaskOverview>) => {
    return Promise.resolve({
        data: true,
        status: StatusCodes.OK
    });
};

/**
 * Get approved and active project tasks overview between date range.
 * @param projectId The project Id of which details to fetch.
 * @param startDate The start date of the date range.
 * @param endDate The end date of the date range.
 */
export const getProjectTasksOverviewAsync = async (
    projectId: string,
    startDate: Date,
    endDate: Date) => {
    return Promise.resolve({
        data: projectTaskOverview,
        status: StatusCodes.OK
    });
};