'use client'

import { useEffect, useState, useContext, FormEvent, Dispatch, SetStateAction } from 'react'
import { useForm, SubmitHandler } from 'react-hook-form'
import { Tooltip } from 'react-tooltip'
import { EyeOpenIcon, EyeClosedIcon } from '@radix-ui/react-icons'

import CustomCheckBox from './element/checkbox'

import { SignIn, SessionInfo, CreateAccount, SessionContextError } from './context/session'

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
    const [isPswrdVisible, setIsPswrdVisible] = useState<boolean>(false)
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
                if (e instanceof SessionContextError) setError('root.serverError', { type: 'already_exists' })
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
                <input type={ isPswrdVisible ? 'text' : 'password' } className='label_value' placeholder='Password' { ...register('password', { required: true }) } />
                <button className='label_char simple' type='button' onClick={() => setIsPswrdVisible(!isPswrdVisible)}>
                    { isPswrdVisible ? <EyeOpenIcon /> : <EyeClosedIcon /> }
                </button>
            </label>
            <div className="check">
                <CustomCheckBox checked={isSignUp} onChange={setIsSignUp} />
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

export default SignInForm