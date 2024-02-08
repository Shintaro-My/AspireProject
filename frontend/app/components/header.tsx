'use client'

import { Popover, Transition } from '@headlessui/react'
import { Fragment } from 'react'

import Link from 'next/link';
import { useEffect, useState, useContext } from 'react';

import { SessionContext, SessionInfo, NullSession, FetchSession, RolesInfo } from '../sessionCC';

import "./header.scss"

type Props = {
    session: SessionInfo,
    rolesInfo: RolesInfo
}
const Login = ({ session, rolesInfo }: Props) => {
    const role = session.role in rolesInfo ? rolesInfo[session.role] : "null"
    if (session == NullSession) {
        return (
            <Popover className="user_info">
                <Popover.Button className='user_info_toggle'>SignIn</Popover.Button>
                <Transition
                    as={Fragment}
                    enterFrom="inactive"
                    leaveTo="inactive"
                    enterTo="active"
                    leaveFrom="active"
                >
                    <Popover.Panel className='user_info_content'>
                        <></>
                    </Popover.Panel>
                </Transition>
            </Popover>
        )
    }
    return (
        <Popover className="user_info">
            <Popover.Button className='user_info_toggle active'>{session.userName}</Popover.Button>
                <Transition
                    as={Fragment}
                    enterFrom="inactive"
                    leaveTo="inactive"
                    enterTo="active"
                    leaveFrom="active"
                >
                    <Popover.Panel className='user_info_content'>
                        <div className="user_info_content_table">
                            <div className="user_info_content_table_key">UserName</div>
                            <div className="user_info_content_table_value">{session.userName}</div>
                            <div className="user_info_content_table_key">Role</div>
                            <div className="user_info_content_table_value">{role}</div>
                            <div className="user_info_content_table_wide">
                                <button className='user_info_toggle'>SignOut</button>
                            </div>
                        </div>
                    </Popover.Panel>
                </Transition>
        </Popover>
    )
}

const Header = () => {
    const sessionContext = useContext(SessionContext)
    const session: SessionInfo = sessionContext?.session ?? NullSession
    const rolesInfo: RolesInfo = sessionContext?.rolesInfo ?? {}

    useEffect(() => {
        if (session == NullSession) {
            FetchSession().then(r => {
                if (r != null && sessionContext != null) {
                    const { setSession } = sessionContext
                    setSession(r)
                }
            })
        }
    }, [])

    return (
        <header className="header">
            <div className="link_wrap">
                <Link href="/">Main</Link>
            </div>
            <Login session={session} rolesInfo={rolesInfo}></Login>
        </header>
    );
};

export default Header;