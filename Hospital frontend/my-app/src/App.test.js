import { render, screen } from '@testing-library/react';
import App from './App';

test('renders login screen heading', () => {
  render(<App />);
  expect(screen.getByText(/Welcome back/i)).toBeInTheDocument();
  expect(screen.getByRole('button', { name: /sign in/i })).toBeInTheDocument();
});
