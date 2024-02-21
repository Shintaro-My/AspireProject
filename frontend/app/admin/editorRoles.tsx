'use client'

import { Dispatch, SetStateAction, useContext, useEffect, useState, Fragment, MouseEventHandler, FormEventHandler } from "react"
import { RadioGroup, Combobox, Transition } from '@headlessui/react'
import { CheckIcon, PersonIcon, CaretSortIcon } from '@radix-ui/react-icons'
import { useForm, SubmitHandler } from "react-hook-form"

import { SessionInfo, NullSession, SessionContext, RolesInfo, UpdateAccount, SessionCCError } from '../sessionCC'
import { getUsers, classNameFormat } from './editor'
import CustomCheckBox from "../components/custom/checkbox"

type RoleFragment = { id: number, label: string }
type RolesFormProps = {
    user: SessionInfo,
    rolesInfo: RolesInfo,
    roleEditable: boolean,
    handler: Dispatch<SetStateAction<number>>
}
export const RolesForm = ({ user, rolesInfo, roleEditable, handler }: RolesFormProps) => {

    let roleFragments: RoleFragment[] = Object.keys(rolesInfo).map(k => {
        const id = Number(k)
        return { id, label: rolesInfo[id] }
    })

    const [query, setQuery] = useState<string>('')
    const [selected, setSelected] = useState<RoleFragment>(roleFragments[0])

    
    useEffect(() => {
        const rf = roleFragments.find(r => r.id == user.role)
        if (rf) {
            setSelected(rf)
        }
    }, [user])

    useEffect(() => {
        handler(selected.id)
    }, [selected])


    const filteredRoles: RoleFragment[] = query == ''
        ? roleFragments
        : roleFragments.filter(r => r.label.toLowerCase().includes(query.toLowerCase()))

    return (
        <>
            <Combobox value={selected} disabled={!roleEditable} onChange={(data) => {
                setSelected(data)
            }}>
                <div className="admin_user_panel_editor_roles">
                    <div className="admin_user_panel_editor_roles_input">
                        <Combobox.Input
                            className="admin_user_panel_editor_roles_input_field"
                            displayValue={(role: RoleFragment) => role.label}
                            onChange={(event) => setQuery(event.target.value)}
                        />
                        <Combobox.Button className="admin_user_panel_editor_roles_input_button">
                            <CaretSortIcon />
                        </Combobox.Button>
                    </div>
                    <div className="admin_user_panel_editor_roles_options">
                        <Transition
                            as={Fragment}
                            leave="transition ease-in duration-100"
                            leaveFrom="opacity-100"
                            leaveTo="opacity-0"
                            afterLeave={() => setQuery('')}
                        >

                            <Combobox.Options className="admin_user_panel_editor_roles_options_list">
                                {filteredRoles.length == 0 && query != '' ? (
                                    <div className="admin_user_panel_editor_roles_options_list_empty">
                                        Nothing found.
                                    </div>
                                ) : (
                                    filteredRoles.map((role, i) => (
                                        <Combobox.Option
                                            key={i}
                                            className={classNameFormat(
                                                'admin_user_panel_editor_roles_options_list_item',
                                                role.id == selected.id && 'selected',
                                                role.id == user.role && 'initial'
                                            )}
                                            value={role}
                                        >
                                            <div className="admin_user_panel_editor_roles_options_list_item_label">{role.label}</div>
                                            
                                            <div className="admin_user_panel_editor_roles_options_list_item_icon">
                                                { role.id == selected.id
                                                    ? <CheckIcon className="admin_user_panel_editor_roles_options_list_item_icon_content" aria-hidden="true" />
                                                    : <span className="admin_user_panel_editor_roles_options_list_item_icon_content" /> }
                                            </div>
                                        </Combobox.Option>
                                    ))
                                )}
                            </Combobox.Options>

                        </Transition>
                    </div>
                </div>
            </Combobox>
        </>
    )
}