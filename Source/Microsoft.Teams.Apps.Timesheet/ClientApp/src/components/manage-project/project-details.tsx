// <copyright file="project-details.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Flex, Text, Status } from '@fluentui/react-northstar';
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import Constants from "../../constants/constants";
import IProjectUtilization from "../../models/project-utilization";
import Donut from "react-donut";

import "react-circular-progressbar/dist/styles.css";
import "./manage-project.scss";

interface IProjectDetailsProps extends WithTranslation {
    projectDetail: IProjectUtilization;
    isMobile: boolean;
}

/**
 * Renders the project details and donut chart.
 * @param props The props with type IProjectDetailsProps.
 */
const ProjectDetails: React.FunctionComponent<IProjectDetailsProps> = props => {
    const localize: TFunction = props.t;
    const [isUtilized, setIsUtilized] = React.useState(false);

    React.useEffect(() => {
        setIsUtilized(props.projectDetail.notUtilizedHours === 0);
    }, [props.projectDetail]);

    /**
     * Get project details.
     */
    const getProjectDetail = () => {
        let projectDetailCard =
            (<div >
                <Flex vAlign="center" gap="gap.large">
                    <Flex.Item >
                        <div className="donut-container">
                            <Donut
                                chartData={[
                                    { name: ` `, data: props.projectDetail.billableHours },
                                    { name: ` `, data: props.projectDetail.nonBillableHours },
                                    { name: ` `, data: props.projectDetail.notUtilizedHours },
                                ]}
                                chartThemeConfig={{
                                    series: {
                                        colors: [Constants.billableStatusColor, Constants.nonBillableStatusColor, Constants.nonUtilizedStatusColor],
                                    },
                                }}
                                showChartLabel={false}
                                chartRadiusRange={[`90%`, `100%`]}
                                chartWidth={300}
                                chartHeight={150}
                                title=""
                                legendAlignment={"top"}
                            />
                            <Flex vAlign="center" hAlign="center">
                                <Text size="medium" content={`${props.projectDetail.billableHours + props.projectDetail.nonBillableHours} ${localize("hoursCapitalLabel")}`} weight="semibold" />
                            </Flex>
                        </div>
                    </Flex.Item>
                    <Flex.Item >
                        <Flex gap="gap.medium">
                            <Flex className="text-container" space="between" column>
                                <Flex vAlign="center" gap="gap.medium">
                                    <Status className="status-bullets" color={Constants.billableStatusColor} title={localize("billable")} />
                                    <Text className={props.isMobile ? "status-label-mobile" : "status-label-web"} size="medium" content={localize("billable")} />
                                </Flex>
                                <Flex vAlign="center" gap="gap.medium">
                                    <Status className="status-bullets" color={Constants.nonBillableStatusColor} title={localize("nonBillable")} />
                                    <Text className={props.isMobile ? "status-label-mobile" : "status-label-web"} content={localize("nonBillable")} />
                                </Flex>
                                <Flex vAlign="center" gap="gap.medium">
                                    <Status className="status-bullets" color={Constants.nonUtilizedStatusColor} title={localize("notUtilized")} />
                                    <Text className={props.isMobile ? "status-label-mobile" : "status-label-web"} content={localize("notUtilized")} />
                                </Flex>
                            </Flex>
                            <Flex className="text-container" space="between" column>
                                <Text className={props.isMobile ? "status-label-mobile" : "status-label-web"} size="medium" content={localize("hours", { hourNumber: props.projectDetail.billableHours })} />
                                <Text className={props.isMobile ? "status-label-mobile" : "status-label-web"} content={localize("hours", { hourNumber: props.projectDetail.nonBillableHours })} />
                                <Text className={props.isMobile ? "status-label-mobile" : "status-label-web"} content={localize("hours", { hourNumber: props.projectDetail.notUtilizedHours })} />
                            </Flex>
                        </Flex>
                    </Flex.Item>
                </Flex>
                <Text size="small" content={isUtilized ? localize("projectUtilizedLabel") : localize("projectUnderutilizedLabel")} />
            </div>);

        return projectDetailCard;
    };
    return (
        <div className="project-details-container" >{getProjectDetail()}</div>
    );
};

export default withTranslation()(ProjectDetails);