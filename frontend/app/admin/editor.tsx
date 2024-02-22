'use client'

import { Dispatch, SetStateAction, useContext, useEffect, useState, Fragment, MouseEventHandler, FormEventHandler } from "react"
import { RadioGroup, Combobox, Transition } from '@headlessui/react'
import { CheckIcon, PersonIcon, CaretSortIcon } from '@radix-ui/react-icons'
import { useForm, SubmitHandler } from "react-hook-form"

import { SessionInfo, NullSession, SessionContext, RolesInfo, UpdateAccount, SessionContextError } from '../components/context/session'
import CustomCheckBox from "../components/element/checkbox"
import { RolesForm } from "./editorRoles"

export const getUsers = async (): Promise<SessionInfo[]> => {
    const req = await fetch('/api/user')
    const users: SessionInfo[] = await req.json()
    return users
        .sort((a, b) => a.role == b.role
            ? (a.userName?.localeCompare(b.userName ?? '') ?? 0)
            : (b.role - a.role)
        )
}

export const classNameFormat = (...args: (string | false | null | undefined)[]): string => args.filter(v => v).join(' ')

type EditorFormType = {
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
    } = useForm<EditorFormType>({ mode: 'onChange' })
    
    const [role, setRole] = useState<number>(0)
    const [userName, setUserName] = useState<string>('')
    const [password, setPassword] = useState<string | null>(null)
    
    useEffect(() => {
        setRole(user.role)
        setUserName(user.userName ?? '')
    }, [user])

    const update = () => getUsers().then(handler)

    const getCurrentUser = (): EditorFormType => {
        return { ...user, userName, password, role }
    }

    const onSubmit: SubmitHandler<EditorFormType> = async (data) => {
        const newUser: EditorFormType = getCurrentUser()
        if (isValid) {
            try {
                await UpdateAccount(newUser.userId ?? '', newUser.userName ?? '', newUser.password || null, newUser.role)
                await update()
            }
            catch(e) {
                if (e instanceof SessionContextError) setError('root.serverError', { type: 'invalid_operation' })
                return
            }
        }
        return
    }

    return (
        <form className="admin_user_panel_editor" onSubmit={handleSubmit(onSubmit)}>
            <RolesForm user={user} rolesInfo={rolesInfo} roleEditable={roleEditable} handler={setRole} />
            <button className="button" type="submit" disabled={user.userId == null}>Update</button>
        </form>
    )
}