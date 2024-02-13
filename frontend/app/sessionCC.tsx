'use client'

import { Dispatch, SetStateAction, createContext, useContext, useState } from "react"

export type SessionInfo = {
    userId: string | null,
    userName: string | null,
    role: number
}

export interface RolesInfo {
    [key: number]: string
}

type SesssionSetter = {
    session: SessionInfo,
    setSession: Dispatch<SetStateAction<SessionInfo>>,
    rolesInfo: RolesInfo
}

export const NullSession: SessionInfo = {
    userId: null,
    userName: null,
    role: -1
}

export const SessionContext = createContext<SesssionSetter | null>(null)

type Props = {
    rolesInfo?: RolesInfo
    children: React.ReactNode
}
export const SessionProvider = ({ children, rolesInfo }: Props) => {
    const [session, setSession] = useState<SessionInfo>(NullSession)
    const value = { session, setSession, rolesInfo: rolesInfo ?? {} }
    return <SessionContext.Provider value={value}>{children}</SessionContext.Provider>
}

export const FetchSession = async (): Promise<SessionInfo | null> => {
    const request = await fetch('/api/auth/session')
    if (!request.ok) {
        return null
    }
    const result = await request.json()
    return result as SessionInfo
}

export const SignIn = async (userName: string, password: string): Promise<SessionInfo | null> => {
    const method = 'POST'
    const headers = {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
    }
    const body = JSON.stringify({ userName, password })
    const request = await fetch('/api/auth', { method, headers, body })
    if (!request.ok) {
        return null
    }
    const result = await request.json()
    return result as SessionInfo
}

export const SignOut = async (): Promise<SessionInfo | null> => {
    const request = await fetch('/api/auth')
    if (!request.ok) {
        return null
    }
    return NullSession
}
