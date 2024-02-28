'use client'

import { useContext } from "react"

import { SessionContext } from '../components/context/session'
import { SSEProvider } from '../components/context/sse'


const Layout = ({ children }: Readonly<{children: React.ReactNode}>) => {
    const sessionContext = useContext(SessionContext)
    const rolesInfo = sessionContext?.rolesInfo ?? {}
    const myRole = sessionContext?.session?.role ?? -1
    const isAnonymous = rolesInfo[myRole] == 'Anonymous' || myRole == -1

    return (
        <SSEProvider isValid={!isAnonymous}>
            {children}
        </SSEProvider>
    )
}

export default Layout