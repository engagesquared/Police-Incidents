import * as React from "react";
import { Flex, Header, Loader, Segment, Divider, List, Text } from "@fluentui/react-northstar";
import { useTranslation } from "react-i18next";
import { useStyles } from "./incidentPage.styles";
import { useParams } from "react-router-dom";
import { IIncidentModel } from "../../models/IIncidentModel";
import { getIncident } from "../../apis/api-list";
import { Person, PersonViewType, PersonCardInteraction } from "@microsoft/mgt-react";

export const IncidentPage = () => {
    const { t } = useTranslation();
    const { id }: { id?: string } = useParams();
    const classes = useStyles();
    const [incident, setIncident] = React.useState<IIncidentModel>();
    React.useEffect(() => {
        (async () => {
            if (!!id) {
                var incident = await getIncident(Number(id));
                setIncident(incident);
            }
        })();
    }, [id]);

    return (
        <div className={classes.container}>
            {!incident && <Loader />}
            {!!incident && (
                <>
                    <Flex>
                        <Header content={t("allIncidentHeader") + ` > ${incident.title}`} as="h3" />
                    </Flex>
                    <Segment color="brand" style={{ borderTopWidth: "5px" }}>
                        <Flex gap="gap.smaller" vAlign="center">
                            <Text content={incident.title} as="h3" />
                            <Text content={incident.description} />
                            <Person
                                userId={incident.incidentManagerId}
                                showPresence={false}
                                view={PersonViewType.oneline}
                                personCardInteraction={PersonCardInteraction.hover}
                            />
                        </Flex>
                        <Divider size={1} />
                        <List
                            items={incident.incidentUpdates.map((iu) => ({
                                content: iu.body,
                                header: iu.title,
                                headerMedia: iu.createdAt,
                                key: iu.id,
                            }))}
                        />
                    </Segment>
                </>
            )}
        </div>
    );
};
