'use client'

import { useContext, useEffect, useState } from "react";
import { RadioGroup } from '@headlessui/react';
import { CheckIcon, PersonIcon } from '@radix-ui/react-icons';

import { SessionInfo, NullSession, SessionContext } from '../sessionCC'

import "./admin.scss"

const getUsers = async (): Promise<SessionInfo[]> => {
    const req = await fetch('/api/user')
    const users: SessionInfo[] = await req.json()
    return users
}

const classNameFormat = (...args: (string | false | null | undefined)[]): string => args.filter(v => v).join(' ')

const AdminPage = () => {
    const [users, setUsers] = useState<SessionInfo[]>([])
    const [selected, setSelected] = useState<SessionInfo | undefined>()
    const sessionContext = useContext(SessionContext)
    const rolesInfo = sessionContext?.rolesInfo ?? {}
    const isMe = (user:SessionInfo): boolean => user.userId == sessionContext?.session?.userId

    useEffect(() => {
        getUsers().then(setUsers)
    }, [])

    return (
        <>
            <h1>Users</h1>
            <RadioGroup value={selected} onChange={setSelected} className='admin_users'>
                {users.map((user: SessionInfo, i: number) => (
                <RadioGroup.Option
                    key={i}
                    value={user}
                    className='admin_users_option'
                >
                    {({ active, checked }) => (
                    <>
                        <div className={classNameFormat('admin_users_option_content', isMe(user) && 'self', active && 'active', checked && 'checked')} title={user.userId ?? ''}>
                            <div className="admin_users_option_content_icon">
                                <PersonIcon />
                            </div>

                            <div className='admin_users_option_content_group'>
                                <RadioGroup.Label
                                    as="p"
                                    className='admin_users_option_content_group_label'
                                >
                                    {user.userName}
                                </RadioGroup.Label>
                                <RadioGroup.Description
                                    as="span"
                                    className='admin_users_option_content_group_desc'
                                >
                                    <div>
                                        {rolesInfo[user.role]}
                                    </div>
                                </RadioGroup.Description>
                            </div>
                            
                            <div className="admin_users_option_content_icon">
                                {checked ? <CheckIcon /> : <></>}
                            </div>
                        </div>
                    </>
                    )}

                </RadioGroup.Option>
                ))}
            </RadioGroup>
        </>
    );
}

export default AdminPage