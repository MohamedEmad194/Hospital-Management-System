import React, { useState, useRef, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { sendChatbotMessage } from '../api/chatbot';

export default function ChatbotWidget() {
    const { t, i18n } = useTranslation();
    const [isOpen, setIsOpen] = useState(false);
    const [messages, setMessages] = useState([
        {
            id: 1,
            text: t('chatbot.welcome'),
            sender: 'bot',
            timestamp: new Date()
        }
    ]);
    const [inputMessage, setInputMessage] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [suggestions, setSuggestions] = useState([]);
    const messagesEndRef = useRef(null);
    const inputRef = useRef(null);
    const widgetRef = useRef(null);

    const scrollToBottom = () => {
        messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    };

    useEffect(() => {
        if (isOpen) {
            scrollToBottom();
            inputRef.current?.focus();
        }
    }, [messages, isOpen]);

    const handleSendMessage = async (messageText = null) => {
        const textToSend = messageText || inputMessage.trim();
        if (!textToSend) return;

        const userMessage = {
            id: Date.now(),
            text: textToSend,
            sender: 'user',
            timestamp: new Date()
        };

        setMessages(prev => [...prev, userMessage]);
        setInputMessage('');
        setIsLoading(true);
        setSuggestions([]);

        try {
            const response = await sendChatbotMessage(textToSend);
            console.log('📨 Chatbot response:', response);
            
            // Handle different response formats
            const responseText = response?.response || response?.Response || response?.message || t('chatbot.noResponse');
            const responseSuggestions = response?.suggestions || response?.Suggestions || [];
            
            const botMessage = {
                id: Date.now() + 1,
                text: responseText,
                sender: 'bot',
                timestamp: new Date()
            };

            setMessages(prev => [...prev, botMessage]);
            setSuggestions(responseSuggestions);
        } catch (error) {
            console.error('❌ Chatbot error details:', error);
            
            // Show more detailed error message
            const errorText = error?.message || error?.response?.data?.message || t('chatbot.error');
            const errorMessage = {
                id: Date.now() + 1,
                text: `${t('chatbot.error')}\n\n${errorText}`,
                sender: 'bot',
                timestamp: new Date()
            };
            setMessages(prev => [...prev, errorMessage]);
        } finally {
            setIsLoading(false);
        }
    };

    const handleKeyPress = (e) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            handleSendMessage();
        }
    };

    const handleSuggestionClick = (suggestion) => {
        handleSendMessage(suggestion);
    };

    const isRtl = i18n.language === 'ar';

    return (
        <>
            {/* Floating Button */}
            {!isOpen && (
                <button
                    onClick={() => setIsOpen(true)}
                    style={{
                        position: 'fixed',
                        bottom: '20px',
                        [isRtl ? 'left' : 'right']: '20px',
                        width: '60px',
                        height: '60px',
                        borderRadius: '50%',
                        background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                        color: 'white',
                        border: 'none',
                        cursor: 'pointer',
                        boxShadow: '0 4px 12px rgba(102, 126, 234, 0.4)',
                        fontSize: '24px',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        zIndex: 1000,
                        transition: 'all 0.3s ease',
                        animation: 'pulse 2s infinite'
                    }}
                    onMouseEnter={(e) => {
                        e.target.style.transform = 'scale(1.1)';
                        e.target.style.boxShadow = '0 6px 16px rgba(102, 126, 234, 0.6)';
                    }}
                    onMouseLeave={(e) => {
                        e.target.style.transform = 'scale(1)';
                        e.target.style.boxShadow = '0 4px 12px rgba(102, 126, 234, 0.4)';
                    }}
                    title={t('chatbot.title')}
                >
                    💬
                </button>
            )}

            {/* Chat Widget */}
            {isOpen && (
                <div
                    ref={widgetRef}
                    style={{
                        position: 'fixed',
                        bottom: '20px',
                        [isRtl ? 'left' : 'right']: '20px',
                        width: '380px',
                        maxWidth: 'calc(100vw - 40px)',
                        height: '600px',
                        maxHeight: 'calc(100vh - 40px)',
                        background: 'white',
                        borderRadius: '16px',
                        boxShadow: '0 8px 32px rgba(0,0,0,0.2)',
                        display: 'flex',
                        flexDirection: 'column',
                        zIndex: 1001,
                        overflow: 'hidden',
                        border: '1px solid #e9ecef'
                    }}
                >
                    {/* Header */}
                    <div style={{
                        background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                        color: 'white',
                        padding: '16px 20px',
                        display: 'flex',
                        justifyContent: 'space-between',
                        alignItems: 'center',
                        borderRadius: '16px 16px 0 0'
                    }}>
                        <div>
                            <h3 style={{ margin: 0, fontSize: '1.1rem', fontWeight: '600' }}>
                                {t('chatbot.title')}
                            </h3>
                            <p style={{ margin: '4px 0 0 0', fontSize: '0.75rem', opacity: 0.9 }}>
                                {t('chatbot.subtitle')}
                            </p>
                        </div>
                        <button
                            onClick={() => setIsOpen(false)}
                            style={{
                                background: 'rgba(255,255,255,0.2)',
                                border: 'none',
                                borderRadius: '50%',
                                width: '32px',
                                height: '32px',
                                color: 'white',
                                cursor: 'pointer',
                                fontSize: '18px',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                transition: 'background 0.2s'
                            }}
                            onMouseEnter={(e) => {
                                e.target.style.background = 'rgba(255,255,255,0.3)';
                            }}
                            onMouseLeave={(e) => {
                                e.target.style.background = 'rgba(255,255,255,0.2)';
                            }}
                        >
                            ×
                        </button>
                    </div>

                    {/* Messages Container */}
                    <div style={{
                        flex: 1,
                        background: '#f8f9fa',
                        overflowY: 'auto',
                        padding: '16px',
                        display: 'flex',
                        flexDirection: 'column',
                        gap: '10px'
                    }}>
                        {messages.map((message) => (
                            <div
                                key={message.id}
                                style={{
                                    display: 'flex',
                                    justifyContent: message.sender === 'user' ? 'flex-end' : 'flex-start'
                                }}
                            >
                                <div style={{
                                    maxWidth: '80%',
                                    padding: '10px 14px',
                                    borderRadius: message.sender === 'user' 
                                        ? (isRtl ? '16px 16px 0 16px' : '16px 16px 16px 0')
                                        : (isRtl ? '16px 16px 16px 0' : '16px 16px 0 16px'),
                                    background: message.sender === 'user'
                                        ? 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)'
                                        : 'white',
                                    color: message.sender === 'user' ? 'white' : '#2d3748',
                                    boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
                                    wordWrap: 'break-word',
                                    whiteSpace: 'pre-wrap',
                                    lineHeight: '1.4',
                                    fontSize: '0.9rem'
                                }}>
                                    {message.text}
                                </div>
                            </div>
                        ))}
                        
                        {isLoading && (
                            <div style={{
                                display: 'flex',
                                justifyContent: 'flex-start'
                            }}>
                                <div style={{
                                    padding: '10px 14px',
                                    borderRadius: isRtl ? '16px 16px 16px 0' : '16px 16px 0 16px',
                                    background: 'white',
                                    boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
                                    display: 'flex',
                                    gap: '4px'
                                }}>
                                    <span style={{ animation: 'blink 1s infinite' }}>●</span>
                                    <span style={{ animation: 'blink 1s infinite 0.2s' }}>●</span>
                                    <span style={{ animation: 'blink 1s infinite 0.4s' }}>●</span>
                                </div>
                            </div>
                        )}

                        <div ref={messagesEndRef} />
                    </div>

                    {/* Suggestions */}
                    {suggestions.length > 0 && (
                        <div style={{
                            padding: '10px 16px',
                            background: '#f8f9fa',
                            borderTop: '1px solid #e9ecef',
                            display: 'flex',
                            flexWrap: 'wrap',
                            gap: '6px',
                            maxHeight: '80px',
                            overflowY: 'auto'
                        }}>
                            {suggestions.map((suggestion, index) => (
                                <button
                                    key={index}
                                    onClick={() => handleSuggestionClick(suggestion)}
                                    style={{
                                        padding: '5px 10px',
                                        background: 'white',
                                        border: '1px solid #667eea',
                                        borderRadius: '16px',
                                        color: '#667eea',
                                        cursor: 'pointer',
                                        fontSize: '0.8rem',
                                        transition: 'all 0.2s',
                                        whiteSpace: 'nowrap'
                                    }}
                                    onMouseEnter={(e) => {
                                        e.target.style.background = '#667eea';
                                        e.target.style.color = 'white';
                                    }}
                                    onMouseLeave={(e) => {
                                        e.target.style.background = 'white';
                                        e.target.style.color = '#667eea';
                                    }}
                                >
                                    {suggestion}
                                </button>
                            ))}
                        </div>
                    )}

                    {/* Input Area */}
                    <div style={{
                        padding: '12px 16px',
                        background: 'white',
                        borderTop: '1px solid #e9ecef',
                        display: 'flex',
                        gap: '8px',
                        alignItems: 'flex-end'
                    }}>
                        <textarea
                            ref={inputRef}
                            value={inputMessage}
                            onChange={(e) => setInputMessage(e.target.value)}
                            onKeyPress={handleKeyPress}
                            placeholder={t('chatbot.placeholder')}
                            disabled={isLoading}
                            style={{
                                flex: 1,
                                padding: '10px 14px',
                                border: '1px solid #e2e8f0',
                                borderRadius: '20px',
                                fontSize: '0.9rem',
                                fontFamily: 'inherit',
                                resize: 'none',
                                minHeight: '40px',
                                maxHeight: '100px',
                                outline: 'none',
                                transition: 'border-color 0.2s',
                                direction: isRtl ? 'rtl' : 'ltr'
                            }}
                            onFocus={(e) => {
                                e.target.style.borderColor = '#667eea';
                            }}
                            onBlur={(e) => {
                                e.target.style.borderColor = '#e2e8f0';
                            }}
                        />
                        <button
                            onClick={() => handleSendMessage()}
                            disabled={isLoading || !inputMessage.trim()}
                            style={{
                                padding: '10px 16px',
                                background: inputMessage.trim() && !isLoading
                                    ? 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)'
                                    : '#cbd5e0',
                                color: 'white',
                                border: 'none',
                                borderRadius: '20px',
                                cursor: inputMessage.trim() && !isLoading ? 'pointer' : 'not-allowed',
                                fontSize: '0.9rem',
                                fontWeight: '500',
                                transition: 'all 0.2s',
                                minWidth: '60px',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center'
                            }}
                            onMouseEnter={(e) => {
                                if (inputMessage.trim() && !isLoading) {
                                    e.target.style.transform = 'translateY(-2px)';
                                }
                            }}
                            onMouseLeave={(e) => {
                                if (inputMessage.trim() && !isLoading) {
                                    e.target.style.transform = 'translateY(0)';
                                }
                            }}
                        >
                            {isLoading ? '⟳' : '➤'}
                        </button>
                    </div>
                </div>
            )}

            <style>{`
                @keyframes blink {
                    0%, 100% { opacity: 0.3; }
                    50% { opacity: 1; }
                }
                @keyframes pulse {
                    0%, 100% { transform: scale(1); }
                    50% { transform: scale(1.05); }
                }
            `}</style>
        </>
    );
}

