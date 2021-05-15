import * as React from 'react';
import { Button, Flex, Text } from '@fluentui/react-northstar';
import { getAccessToken } from "../../apis/api-list";
import './home.scss';
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";

export interface IHomeProps extends WithTranslation {
}

export interface IHomeState {
    token:string;
}

class Home extends React.Component<IHomeProps, IHomeState> {
    localize: TFunction;
    constructor(props: IHomeProps) {
        super(props);
        this.localize = this.props.t;

        this.state = {
            token:""
        };
    }

    public componentDidMount = () => {
    }

    public componentWillUnmount = () => {
    }

    private onLoadTokenClick = async () => {
        var result = await getAccessToken();
        this.setState({
            token: result.data
        })
    }

    public render(): JSX.Element {
        return (

            <div className={"emptydiv"}>
                <Flex column hAlign="center">
                    <Text content={this.localize('welcomeMessage')} />
                    <Text content={this.localize('getStarted')} />
                    <Button content={this.localize('getTokenBtnText')} onClick={() => { this.onLoadTokenClick(); }} primary />
                    <Text content={this.state.token} />
                </Flex>
            </div>

        );
    }
}
export default withTranslation()(Home) 