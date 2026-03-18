import apiClient from './client';

export const sendChatbotMessage = async (messageText) => {
    const fullUrl = apiClient.defaults.baseURL + '/chatbot/message';
    console.log('💬 Sending chatbot message to:', fullUrl);
    console.log('📝 Message text:', messageText);
    
    // Ensure we send the correct format
    const payload = { message: messageText };
    console.log('📦 Payload:', payload);
    
    try {
        const response = await apiClient.post('/chatbot/message', payload);
        console.log('✅ Chatbot response received:', response.data);
        console.log('✅ Response structure:', {
            hasResponse: !!response.data?.response,
            hasResponseProperty: !!response.data?.Response,
            hasSuggestions: !!response.data?.suggestions,
            fullData: response.data
        });
        return response.data;
    } catch (error) {
        console.error('❌ Chatbot error:', error);
        console.error('❌ Error response:', error?.response?.data);
        console.error('❌ Error status:', error?.response?.status);
        console.error('❌ Error status text:', error?.response?.statusText);
        console.error('❌ Error URL:', error?.config?.url);
        console.error('❌ Full error:', JSON.stringify(error, null, 2));
        
        // Throw a more descriptive error
        let errorMessage = 'Failed to send message';
        if (error?.response?.data) {
            if (typeof error.response.data === 'string') {
                errorMessage = error.response.data;
            } else if (error.response.data.message) {
                errorMessage = error.response.data.message;
            } else if (error.response.data.error) {
                errorMessage = error.response.data.error;
            }
        } else if (error?.message) {
            errorMessage = error.message;
        }
        
        throw new Error(errorMessage);
    }
};

