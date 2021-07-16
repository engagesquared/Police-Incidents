import * as React from "react";
import { Flex, Text, Button, ChevronDownIcon } from "@fluentui/react-northstar";
import { useStyles } from "./incidentUpdates.styles";
import { IIncidentUpdateModel } from "../../models";
import { Person, PersonViewType, PersonCardInteraction } from "@microsoft/mgt-react";
import { formatDateTime } from "../../utils";
import { useGlobalState } from "../../hooks/useGlobalState";

export interface IIncidentUpdatesProps {
    itemsToShow: number;
    updates: IIncidentUpdateModel[];
}

export const IncidentUpdates = (props: IIncidentUpdatesProps) => {
    const classes = useStyles();
    const { state } = useGlobalState();
    const [itemsToShow, setItemsToShow] = React.useState<IIncidentUpdateModel[]>([]);
    React.useEffect(() => {
        setItemsToShow(props.updates.slice(0, props.itemsToShow));
    }, [props.updates, props.itemsToShow]);

    const showMoreBtn = itemsToShow.length < props.updates.length;
    const showLessBtn = itemsToShow.length === props.updates.length && itemsToShow.length > props.itemsToShow;

    const onShowMoreClick = () => {
        setItemsToShow(props.updates);
    };

    const onShowLessClick = () => {
        setItemsToShow(props.updates.slice(0, props.itemsToShow));
    };

    return (
        <div className={classes.content}>
            {itemsToShow.length === 0 && (
                <div className={classes.item}>
                    <Text>No updates</Text>
                </div>
            )}
            {itemsToShow.length > 0 &&
                itemsToShow.map((i) => (
                    <Flex key={i.id} column gap="gap.smaller" className={classes.item}>
                        <Text weight="semibold">{i.title}</Text>
                        <Text className={classes.text}>{i.body}</Text>
                        <Flex className={classes.itemBottom} vAlign="center">
                            <Text>{formatDateTime(state.teamsContext.locale, i.createdAt)}</Text>
                            <Person userId={i.createdById} showPresence={false} view={PersonViewType.oneline} personCardInteraction={PersonCardInteraction.hover} />
                        </Flex>
                    </Flex>
                ))}
            {showMoreBtn && (
                <Flex hAlign="center">
                    <Button size="small" icon={<ChevronDownIcon size="small" />} primary text content="Show more" onClick={onShowMoreClick} />
                </Flex>
            )}
            {showLessBtn && (
                <Flex hAlign="center">
                    <Button
                        size="small"
                        icon={<ChevronDownIcon className={classes.showLessIcon} size="small" />}
                        primary
                        text
                        content="Show less"
                        onClick={onShowLessClick}
                    />
                </Flex>
            )}
        </div>
    );
};
