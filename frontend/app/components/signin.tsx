'use client'

import { useEffect, useState, useContext, FormEvent, Dispatch, SetStateAction } from 'react'
import { useForm, SubmitHandler } from 'react-hook-form'
import { Tooltip } from 'react-tooltip'
import { Switch } from '@headlessui/react'

import { SignIn, SessionInfo, CreateAccount, SessionCCError } from '../sessionCC'

type UserFormType = {
    username: string,
    password: string
}

type Props = {
    handler?: Dispatch<SetStateAction<SessionInfo>> | ((session: SessionInfo) => any),
    className?: string
}
const SignInForm = ({ handler, className }: Props) => {
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
    } = useForm<UserFormType>({ mode: 'onChange' })
    const onSubmit: SubmitHandler<UserFormType> = async (data) => {
        if (isSignUp) {
            try {
                await CreateAccount(data.username, data.password)
            } catch(e) {
                if (e instanceof SessionCCError) setError('root.serverError', { type: 'already_exists' })
                return
            }
        }
        const session = await SignIn(data.username, data.password)
        if (session) {
            handler?.(session)
            console.log(`Signed In! Welcome, ${session.userName}!`)
        }
        else {
            setValue('password', '')
            setError('root.serverError', { type: 'unauth' })
        }
        return
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
            <div className="check">
                <Switch checked={isSignUp} onChange={setIsSignUp} className={`check_box${isSignUp ? ' checked' : ''}`}>
                    <div className={`check_box_toggle${isSignUp ? ' checked' : ''}`} area-hidden='true'></div>
                </Switch>
                <div className="check_label">
                    <span className={isSignUp ? '' : 'bold'}>SignIn</span> or <span className={isSignUp ? 'bold' : ''}>Create</span>
                </div>
            </div>
            <div className="buttons">
                <button type='submit' disabled={!isValid || isSubmitting} className={isSignUp ? 'sub': ''}>{isSignUp ? 'Create Account' : 'SignIn'}</button>
            </div>
            
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