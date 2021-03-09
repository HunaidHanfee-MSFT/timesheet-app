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
    const [isOverutilized, setIsOverutilized] = React.useState(false);
    const [isBillableOverutilized, setIsBillableOverutilized] = React.useState(false);
    const [isNonBillableOverutilized, setIsNonBillableOverutilized] = React.useState(false);

    React.useEffect(() => {
        setIsUtilized(props.projectDetail.totalHours === props.projectDetail.billableUtilizedHours + props.projectDetail.nonBillableUtilizedHours);
        setIsOverutilized(props.projectDetail.totalHours < props.projectDetail.billableUtilizedHours + props.projectDetail.nonBillableUtilizedHours);
        setIsNonBillableOverutilized(props.projectDetail.nonBillableUnderutilizedHours < 0);
        setIsBillableOverutilized(props.projectDetail.billableUnderutilizedHours < 0);
    }, [props.projectDetail]);

    /**
     * Get utilization status.
     */
    const getUtilizationStatus = () => {
        let total = props.projectDetail.totalHours;
        let totalUtilized = props.projectDetail.billableUtilizedHours + props.projectDetail.nonBillableUtilizedHours;

        if (totalUtilized > total && !isUtilized) {
            return localize("overUtilizedLabel");
        }
        else if (totalUtilized < total) {
            return localize("underutilizedLabel");
        }
        else if (totalUtilized === total) {
            return localize("fullyUtilizedLabel");
        }
    }

    /**
     * Get view for web.
     */
    const getWebView = () => {
        return <Flex gap="gap.small">
            <Flex className="text-container" space="between" column>
                <Flex vAlign="center" gap="gap.medium" >
                    <Status className="status-bullets" color={Constants.billableStatusColor} title={localize("billableUtilized")} />
                    <Text className={props.isMobile ? "status-label-mobile" : "status-label-web"} size="medium" content={localize("billableUtilized")} />
                </Flex>
                <Flex vAlign="center" gap="gap.medium">
                    <Status className="status-bullets" color={Constants.underUtilizedBillableStatusColor} title={localize("billableUnutilized")} />
                    <Text className={props.isMobile ? "status-label-mobile" : "status-label-web"} content={localize("billableUnutilized")} />
                </Flex>
                <Flex vAlign="center" gap="gap.medium">
                    <Status className="status-bullets" color={Constants.nonBillableStatusColor} title={localize("nonBillableUtilized")} />
                    <Text className={props.isMobile ? "status-label-mobile" : "status-label-web"} content={localize("nonBillableUtilized")} />
                </Flex>
                <Flex vAlign="center" gap="gap.medium">
                    <Status className="status-bullets" color={Constants.underutilizedNonUtilizedStatusColor} title={localize("nonBillableUnutilized")} />
                    <Text className={props.isMobile ? "status-label-mobile" : "status-label-web"} content={localize("nonBillableUnutilized")} />
                </Flex>
            </Flex>
            <Flex className="text-container" space="between" column>
                <Text truncated className={props.isMobile ? "status-label-mobile" : "status-label-web"} size="medium" content={localize("hours", { hourNumber: props.projectDetail.billableUtilizedHours })} />
                <Text truncated className={props.isMobile ? "status-label-mobile" : "status-label-web"} content={localize("hours", { hourNumber: props.projectDetail.billableUnderutilizedHours })} />
                <Text truncated className={props.isMobile ? "status-label-mobile" : "status-label-web"} content={localize("hours", { hourNumber: props.projectDetail.nonBillableUtilizedHours })} />
                <Text truncated className={props.isMobile ? "status-label-mobile" : "status-label-web"} content={localize("hours", { hourNumber: props.projectDetail.nonBillableUnderutilizedHours })} />
            </Flex>
        </Flex>
    }

    /**
     * Get view to show for mobile.
     */
    const getMobileView = () => {
        return <Flex gap="gap.small">
            <Flex className="text-container mobile-text" space="between" column>
                <Flex vAlign="center" >
                    <Flex vAlign="center" gap="gap.medium">
                        <Status className="status-bullets" color={Constants.billableStatusColor} title={localize("billableUtilized")} />
                        <Text className={props.isMobile ? "status-label-mobile" : "status-label-web"} size="medium" content={`${localize("billableUtilized")}`} />
                    </Flex>
                    <Flex.Item push>
                        <Text className={props.isMobile ? "status-label-mobile" : "status-label-web"} size="medium" content={localize("hours", { hourNumber: props.projectDetail.billableUtilizedHours })} />
                    </Flex.Item>
                </Flex>
                <Flex vAlign="center">
                    <Flex vAlign="center" gap="gap.medium">
                        <Status className="status-bullets" color={Constants.underUtilizedBillableStatusColor} title={localize("billableUnutilized")} />
                        <Text className={props.isMobile ? "status-label-mobile" : "status-label-web"} content={`${localize("billableUnutilized")}`} />
                    </Flex>
                    <Flex.Item push>
                        <Text className={props.isMobile ? "status-label-mobile" : "status-label-web"} content={localize("hours", { hourNumber: props.projectDetail.billableUnderutilizedHours })} />
                    </Flex.Item>
                </Flex>
                <Flex vAlign="center">
                    <Flex vAlign="center" gap="gap.medium">
                        <Status className="status-bullets" color={Constants.nonBillableStatusColor} title={localize("nonBillableUtilized")} />
                        <Text className={props.isMobile ? "status-label-mobile" : "status-label-web"} content={`${localize("nonBillableUtilized")}`} />
                    </Flex>
                    <Flex.Item push>
                        <Text className={props.isMobile ? "status-label-mobile" : "status-label-web"} content={localize("hours", { hourNumber: props.projectDetail.nonBillableUtilizedHours })} />
                    </Flex.Item>
                </Flex>
                <Flex vAlign="center">
                    <Flex vAlign="center" gap="gap.medium">
                        <Status className="status-bullets" color={Constants.underutilizedNonUtilizedStatusColor} title={localize("nonBillableUnutilized")} />
                        <Text className={props.isMobile ? "status-label-mobile" : "status-label-web"} content={`${localize("nonBillableUnutilized")}`} />
                    </Flex>
                    <Flex.Item push>
                        <Text className={props.isMobile ? "status-label-mobile" : "status-label-web"} content={localize("hours", { hourNumber: props.projectDetail.nonBillableUnderutilizedHours })} />
                    </Flex.Item>
                </Flex>
            </Flex>
        </Flex>
    }

    /**
     * Get project details.
     */
    const getProjectDetail = () => {
        let projectDetailCard =
            <div >
                <Flex vAlign="center">
                    <Flex.Item >
                        <div className={!props.isMobile ? "donut-container" : "donut-container-mobile"}>
                            {isOverutilized || isBillableOverutilized || isNonBillableOverutilized ?
                                <Flex hAlign="center" vAlign="center" className="total-target-label" column>
                                    <Text weight="semibold" size="medium" content={getUtilizationStatus()} className={isUtilized ? "project-meets-target" : "project-below-target"} />
                                </Flex>
                                :
                                <Donut
                                    chartData={[
                                        { name: ` `, data: props.projectDetail.billableUtilizedHours },
                                        { name: ` `, data: props.projectDetail.billableUnderutilizedHours },
                                        { name: ` `, data: props.projectDetail.nonBillableUtilizedHours },
                                        { name: ` `, data: props.projectDetail.nonBillableUnderutilizedHours },
                                    ]}
                                    chartThemeConfig={{
                                        chart: {
                                            background: "transparent",
                                        },
                                        series: {
                                            colors: [Constants.billableStatusColor, Constants.underUtilizedBillableStatusColor, Constants.nonBillableStatusColor, Constants.underutilizedNonUtilizedStatusColor],
                                        },
                                        chartExportMenu: {
                                            backgroundColor: "transparent",
                                            color: "transparent"
                                        }
                                    }}
                                    showChartLabel={false}
                                    chartRadiusRange={[`90%`, `100%`]}
                                    chartWidth={!props.isMobile ? 250 : 200}
                                    chartHeight={!props.isMobile ? 200 : 170}
                                    title=""
                                    legendAlignment={"top"}
                                />}
                            <Flex vAlign="center" hAlign="center" column>
                                {!isOverutilized && !isBillableOverutilized && !isNonBillableOverutilized && <Text weight="semibold" size="medium" content={getUtilizationStatus()} className={isUtilized ? "project-meets-target" : "project-below-target"} />}
                            </Flex>
                        </div>
                    </Flex.Item>
                    <Flex.Item >
                        {props.isMobile ? getMobileView() : getWebView}
                    </Flex.Item>
                </Flex>
            </div>;

        return projectDetailCard;
    };
    return (
        <div className="project-details-container" >{getProjectDetail()}</div>
    );
};

export default withTranslation()(ProjectDetails);