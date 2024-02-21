'use client'

import { useContext, useEffect, useState } from "react"

import { SessionInfo, NullSession, SessionContext } from '../sessionCC'


const Layout = ({ children }: Readonly<{children: React.ReactNode}>) => {
    const sessionContext = useContext(SessionContext)
    const rolesInfo = sessionContext?.rolesInfo ?? {}
    const myRole = sessionContext?.session?.role ?? -1
    if (rolesInfo[myRole] != 'Administrator') {
        return (
            <>
                <h1>Oops!</h1>
                <h3>You have no access to this page!</h3>
            </>
        )
    }
    return (
        <div className="admin">
            {children}
        </div>
    )
}

export default Layout