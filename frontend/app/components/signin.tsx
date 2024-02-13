'use client'

import { useEffect, useState, useContext, FormEvent, Dispatch, SetStateAction } from 'react'
import { useForm, SubmitHandler } from 'react-hook-form'
import { Tooltip } from 'react-tooltip'

import { SignIn, SessionInfo, NullSession, FetchSession, RolesInfo } from '../sessionCC'

type UserFormType = {
    username: string,
    password: string
}

type Props = {
    handler?: Dispatch<SetStateAction<SessionInfo>> | ((session: SessionInfo) => any),
    className?: string
}
const SignInForm = ({ handler, className }: Props) => {
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
    } = useForm<UserFormType>({ mode: 'onChange' })
    const onSubmit: SubmitHandler<UserFormType> = async (data) => {
        const session = await SignIn(data.username, data.password)
        if (session) {
            handler?.(session)
            console.log(`Signed In! Welcome, ${session.userName}!`)
        }
        else {
            setValue('password', '')
            setError('root.serverError', { type: 'unauth' })
        }
    }
    return (
        <form onSubmit={handleSubmit(onSubmit)} className={className}>
            <label className='label'>
                <a data-tooltip-id='usrnm'></a>
                <span className='label_char'>@</span>
                <input type="text" className='label_value' placeholder='Username' { ...register('username', { required: true }) } />
            </label>
            <label className='label'>
                <a data-tooltip-id='pswrd'></a>
                <input type="password" className='label_value' placeholder='Password' { ...register('password', { required: true }) } />
            </label>
            <button type='submit' disabled={!isValid || isSubmitting}>SignIn</button>
            
            {errors.username && (
                <Tooltip
                    id='usrnm'
                    place='left'
                    defaultIsOpen={true}
                >
                    <span className='label_error'>Required.</span>
                </Tooltip>
            )}
            {(errors.password || errors.root?.serverError.type == 'unauth') && (
                <Tooltip
                    id='pswrd'
                    place='left'
                    defaultIsOpen={true}
                >
                    {errors.password && (<span className='label_error'>Required.</span>)}
                    {errors.root?.serverError.type == 'unauth' && (<span className='label_error'>Unauthorized.</span>)}
                </Tooltip>
            )}
        </form>
    )
}

export default SignInForm;