import React, { useState, useRef, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { sendChatbotMessage } from '../api/chatbot';

export default function Chatbot() {
    const { t, i18n } = useTranslation();
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

    const scrollToBottom = () => {
        messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    };

    useEffect(() => {
        scrollToBottom();
    }, [messages]);

    const handleSendMessage = async (messageText = null) => {
        const textToSend = messageText || inputMessage.trim();
        if (!textToSend) return;

        // Add user message
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
            const response = await sendChatbotMessage({ message: textToSend });
            
            // Add bot response
            const botMessage = {
                id: Date.now() + 1,
                text: response.response,
                sender: 'bot',
                timestamp: new Date()
            };

            setMessages(prev => [...prev, botMessage]);
            setSuggestions(response.suggestions || []);
        } catch (error) {
            const errorMessage = {
                id: Date.now() + 1,
                text: t('chatbot.error'),
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
        <div style={{
            maxWidth: '1200px',
            margin: '0 auto',
            padding: '20px',
            height: 'calc(100vh - 90px)',
            display: 'flex',
            flexDirection: 'column'
        }}>
            {/* Header */}
            <div style={{
                background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                color: 'white',
                padding: '20px',
                borderRadius: '12px 12px 0 0',
                boxShadow: '0 4px 6px rgba(0,0,0,0.1)'
            }}>
                <h1 style={{ margin: 0, fontSize: '1.5rem', fontWeight: '600' }}>
                    {t('chatbot.title')}
                </h1>
                <p style={{ margin: '8px 0 0 0', opacity: 0.9, fontSize: '0.9rem' }}>
                    {t('chatbot.subtitle')}
                </p>
            </div>

            {/* Messages Container */}
            <div style={{
                flex: 1,
                background: '#f8f9fa',
                overflowY: 'auto',
                padding: '20px',
                borderLeft: '1px solid #e9ecef',
                borderRight: '1px solid #e9ecef',
                display: 'flex',
                flexDirection: 'column',
                gap: '12px'
            }}>
                {messages.map((message) => (
                    <div
                        key={message.id}
                        style={{
                            display: 'flex',
                            justifyContent: message.sender === 'user' ? 'flex-end' : 'flex-start',
                            marginBottom: '8px'
                        }}
                    >
                        <div style={{
                            maxWidth: '70%',
                            padding: '12px 16px',
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
                            lineHeight: '1.5'
                        }}>
                            {message.text}
                            <div style={{
                                fontSize: '0.75rem',
                                opacity: 0.7,
                                marginTop: '4px',
                                textAlign: message.sender === 'user' ? 'right' : 'left'
                            }}>
                                {new Date(message.timestamp).toLocaleTimeString(i18n.language, {
                                    hour: '2-digit',
                                    minute: '2-digit'
                                })}
                            </div>
                        </div>
                    </div>
                ))}
                
                {isLoading && (
                    <div style={{
                        display: 'flex',
                        justifyContent: 'flex-start'
                    }}>
                        <div style={{
                            padding: '12px 16px',
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
                    padding: '12px 20px',
                    background: '#f8f9fa',
                    borderTop: '1px solid #e9ecef',
                    borderLeft: '1px solid #e9ecef',
                    borderRight: '1px solid #e9ecef',
                    display: 'flex',
                    flexWrap: 'wrap',
                    gap: '8px',
                    maxHeight: '100px',
                    overflowY: 'auto'
                }}>
                    {suggestions.map((suggestion, index) => (
                        <button
                            key={index}
                            onClick={() => handleSuggestionClick(suggestion)}
                            style={{
                                padding: '6px 12px',
                                background: 'white',
                                border: '1px solid #667eea',
                                borderRadius: '20px',
                                color: '#667eea',
                                cursor: 'pointer',
                                fontSize: '0.85rem',
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
                padding: '16px 20px',
                background: 'white',
                border: '1px solid #e9ecef',
                borderRadius: '0 0 12px 12px',
                boxShadow: '0 -2px 4px rgba(0,0,0,0.05)',
                display: 'flex',
                gap: '12px',
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
                        padding: '12px 16px',
                        border: '1px solid #e2e8f0',
                        borderRadius: '24px',
                        fontSize: '0.95rem',
                        fontFamily: 'inherit',
                        resize: 'none',
                        minHeight: '44px',
                        maxHeight: '120px',
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
                        padding: '12px 24px',
                        background: inputMessage.trim() && !isLoading
                            ? 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)'
                            : '#cbd5e0',
                        color: 'white',
                        border: 'none',
                        borderRadius: '24px',
                        cursor: inputMessage.trim() && !isLoading ? 'pointer' : 'not-allowed',
                        fontSize: '0.95rem',
                        fontWeight: '500',
                        transition: 'all 0.2s',
                        minWidth: '80px',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        gap: '6px'
                    }}
                    onMouseEnter={(e) => {
                        if (inputMessage.trim() && !isLoading) {
                            e.target.style.transform = 'translateY(-2px)';
                            e.target.style.boxShadow = '0 4px 12px rgba(102, 126, 234, 0.4)';
                        }
                    }}
                    onMouseLeave={(e) => {
                        if (inputMessage.trim() && !isLoading) {
                            e.target.style.transform = 'translateY(0)';
                            e.target.style.boxShadow = 'none';
                        }
                    }}
                >
                    {isLoading ? (
                        <>
                            <span style={{ animation: 'spin 1s linear infinite' }}>⟳</span>
                            {t('chatbot.sending')}
                        </>
                    ) : (
                        <>
                            {t('chatbot.send')}
                            <span style={{ marginLeft: '4px' }}>➤</span>
                        </>
                    )}
                </button>
            </div>

            <style>{`
                @keyframes blink {
                    0%, 100% { opacity: 0.3; }
                    50% { opacity: 1; }
                }
                @keyframes spin {
                    from { transform: rotate(0deg); }
                    to { transform: rotate(360deg); }
                }
            `}</style>
        </div>
    );
}

