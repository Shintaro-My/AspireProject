'use client'

import { Popover, Transition } from '@headlessui/react'
import { Dispatch, Fragment, MouseEvent, SetStateAction } from 'react'

import Link from 'next/link'
import { useEffect, useState, useContext } from 'react'

import { SessionContext, SessionInfo, NullSession, FetchSession, RolesInfo, SignOut } from './context/session'

import SignInForm from './signin'

import "./header.scss"

type Props = {
    session: SessionInfo,
    rolesInfo: RolesInfo,
    handler?: Dispatch<SetStateAction<SessionInfo>>
}
const CheckSession = ({ session, rolesInfo, handler }: Props) => {
    const [open, setOpen] = useState<boolean>()
    const [disable, setDisable] = useState<boolean>(false)
    const role = session.role in rolesInfo ? rolesInfo[session.role] : "null"
    if (session == NullSession || session.userId == null) {
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
                        <SignInForm handler={handler} className='user_info_content_form'></SignInForm>
                    </Popover.Panel>
                </Transition>
            </Popover>
        )
    }
    const onClick = async (e: MouseEvent<HTMLButtonElement>) => {
        setDisable(true)
        if (confirm('Are you sure you want to sign out this page?')) {
            await SignOut()
            handler?.(NullSession)
        }
        setDisable(false)
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
                                <button onClick={onClick} disabled={disable}>SignOut</button>
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
                <Link href="/admin">Admin</Link>
            </div>
            <CheckSession session={session} rolesInfo={rolesInfo} handler={sessionContext?.setSession}></CheckSession>
        </header>
    )
}

export default Header