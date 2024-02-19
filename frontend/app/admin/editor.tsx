'use client'

import { Dispatch, SetStateAction, useContext, useEffect, useState, Fragment, MouseEventHandler, FormEventHandler } from "react";
import { RadioGroup, Combobox, Transition } from '@headlessui/react';
import { CheckIcon, PersonIcon, CaretSortIcon } from '@radix-ui/react-icons';

import { SessionInfo, NullSession, SessionContext, RolesInfo, UpdateAccount, SessionCCError } from '../sessionCC'
import { useForm, SubmitHandler } from "react-hook-form";

export const getUsers = async (): Promise<SessionInfo[]> => {
    const req = await fetch('/api/user')
    const users: SessionInfo[] = await req.json()
    return users.sort((a, b) => a.role == b.role ? (a.userName?.localeCompare(b.userName ?? '') ?? 0) : (b.role - a.role))
}

export const classNameFormat = (...args: (string | false | null | undefined)[]): string => args.filter(v => v).join(' ')

type RoleFragment = { id: number, label: string }
type RolesFormProps = {
    user: SessionInfo,
    rolesInfo: RolesInfo,
    roleEditable: boolean,
    handler: Dispatch<SetStateAction<number>>
}
const RolesForm = ({ user, rolesInfo, roleEditable, handler }: RolesFormProps) => {

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

type UserEditorFormType = {
    userId: string | null,
    userName: string | null,
    role: number,
    password: string | null
}

export type EditorProps = {
    user: SessionInfo,
    rolesInfo: RolesInfo,
    roleEditable: boolean,
    handler: Dispatch<SetStateAction<SessionInfo[]>>
}
export const EditorForm = ({ user, rolesInfo, roleEditable, handler }: EditorProps) => {
    
    const [isSignUp, setIsSignUp] = useState<boolean>(false)
    const {
        handleSubmit,
        register,
        formState: {
            errors,
            isValid,
            isSubmitting
        },
        setValue,
        setError
    } = useForm<UserEditorFormType>({ mode: 'onChange' })
    
    const [role, setRole] = useState<number>(0)
    const [userName, setUserName] = useState<string>('')
    const [password, setPassword] = useState<string | null>(null)
    
    useEffect(() => {
        setRole(user.role)
        setUserName(user.userName ?? '')
    }, [user])

    const update = () => getUsers().then(handler)

    const getCurrentUser = (): UserEditorFormType => {
        return { ...user, userName, password, role }
    }

    const onSubmit: SubmitHandler<UserEditorFormType> = async (data) => {
        const newUser: UserEditorFormType = getCurrentUser()
        if (isValid) {
            try {
                await UpdateAccount(newUser.userId ?? '', newUser.userName ?? '', newUser.password, newUser.role)
                await update()
            }
            catch(e) {
                if (e instanceof SessionCCError) setError('root.serverError', { type: 'invalid_operation' })
                return
            }
        }
        return
    }

    return (
        <form className="admin_user_panel_editor" onSubmit={handleSubmit(onSubmit)}>
            <RolesForm user={user} rolesInfo={rolesInfo} roleEditable={roleEditable} handler={setRole} />
            <pre style={{display: 'inline-block'}}>{JSON.stringify(getCurrentUser(), null, 2)}</pre>
            <button type="submit">Update</button>
        </form>
    )
}