'use client'

import { Dispatch, SetStateAction, useContext, useEffect, useState, Fragment, use } from "react"
import { RadioGroup, Combobox, Transition } from '@headlessui/react'
import { CheckIcon, EyeNoneIcon, PersonIcon, StarIcon, StarFilledIcon, Cross1Icon } from '@radix-ui/react-icons'

import { SessionInfo, NullSession, SessionContext, RolesInfo } from '../sessionCC'
import { getUsers, classNameFormat, EditorForm } from "./editor"

import "./admin.scss"


const AdminPage = () => {
    const [users, setUsers] = useState<SessionInfo[]>([])
    const [selected, setSelected] = useState<SessionInfo>(NullSession)
    const sessionContext = useContext(SessionContext)
    const rolesInfo = sessionContext?.rolesInfo ?? {}
    const isMe = (user:SessionInfo): boolean => user.userId == sessionContext?.session?.userId

    const getRoleIcons = (n: number) => {
        const icons = [<EyeNoneIcon />, <PersonIcon />, <StarIcon />, <StarFilledIcon />]
        return icons[n] ?? <></>
    }

    useEffect(() => {
        getUsers().then(setUsers)
    }, [])

    useEffect(() => {
        setSelected(NullSession)
    }, [users])

    return (
        <>
            <h1>Users</h1>
            <div className="admin_user">
                <RadioGroup value={selected} onChange={setSelected} className='admin_user_users'>
                    {users.map((user: SessionInfo, i: number) => (
                    <RadioGroup.Option
                        key={i}
                        value={user}
                        className='admin_user_users_option'
                    >
                        {({ active, checked }) => (
                        <>
                            <div className={classNameFormat('admin_user_users_option_content', isMe(user) && 'self', active && 'active', checked && 'checked')} title={user.userId ?? ''}>
                                <div className="admin_user_users_option_content_icon">
                                    { getRoleIcons(user.role) }
                                </div>

                                <div className='admin_user_users_option_content_group'>
                                    <RadioGroup.Label
                                        as="p"
                                        className='admin_user_users_option_content_group_label'
                                    >
                                        {user.userName}
                                    </RadioGroup.Label>
                                    <RadioGroup.Description
                                        as="span"
                                        className='admin_user_users_option_content_group_desc'
                                    >
                                        <div>
                                            {rolesInfo[user.role]}
                                        </div>
                                    </RadioGroup.Description>
                                </div>
                                
                                <div className="admin_user_users_option_content_icon">
                                    {checked ? <CheckIcon /> : <></>}
                                </div>
                            </div>
                        </>
                        )}

                    </RadioGroup.Option>
                    ))}
                </RadioGroup>
                    <div className="admin_user_panel">
                        { selected.userId ? (
                            <>
                                <button className="simple close" onClick={() => setSelected(NullSession)}>
                                    <Cross1Icon />
                                </button>
                                <EditorForm user={selected} rolesInfo={rolesInfo} roleEditable={!isMe(selected)} handler={setUsers} />
                            </>
                        ) : (
                            <div className="admin_user_panel_empty">
                                <h2>Select User...</h2>
                            </div>
                        ) }
                    </div>
            </div>
        </>
    )
}

export default AdminPage