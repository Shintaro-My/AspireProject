'use client'

import { Dispatch, SetStateAction, createContext, useContext, useState } from "react"
import { SSEContext } from '../../context/sse'
import './sselog.scss'

import SimpleBar from 'simplebar-react';
import 'simplebar-react/dist/simplebar.min.css';



const dateFormatter = new Intl.DateTimeFormat('ja-JP', {
    dateStyle: 'medium',
    timeStyle: 'medium'
})
export const SSELogElement = () => {
    const sseContext = useContext(SSEContext)
    const ping = sseContext?.ping
    const msgs = sseContext?.messages ?? []

    const date = ping ? dateFormatter.format(new Date(ping)) : '-'

    return (
        <>
        <div className='sselog'>
            <div className="sselog_ago">
                <div className="sselog_ago_label">LastUpdate:</div>
                <div className="sselog_ago_value">{date}</div>
            </div>
            { msgs.map((msg, i) => (
                <SimpleBar className="sselog_msg" key={i}>{JSON.stringify(msg)}</SimpleBar>
            )) }
        </div>
        </>
    )
}