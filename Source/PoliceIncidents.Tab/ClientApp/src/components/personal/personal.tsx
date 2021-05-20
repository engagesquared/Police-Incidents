import * as React from "react";
import { Button, Flex, Grid, Segment, Header } from "@fluentui/react-northstar";
import "./personal.scss";
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";

export interface IPersonalProps extends WithTranslation {}

export interface IPersonalState {}

class Personal extends React.Component<IPersonalProps, IPersonalState> {
    localize: TFunction;
    constructor(props: IPersonalProps) {
        super(props);
        this.localize = this.props.t;

        this.state = {};
    }

    public componentDidMount = () => {};

    public componentWillUnmount = () => {};

    public render(): JSX.Element {
        return (
            <Flex column>
                <Header as="h2" content="My Incidents" />
                <Segment color="brand"></Segment>
                <br />
                <Segment color="brand"></Segment>
            </Flex>
        );
    }
}
export default withTranslation()(Personal);
