package com.capstone.Arogya.service;

import com.capstone.Arogya.dto.AuthRequest;
import com.capstone.Arogya.dto.AuthResponse;
import com.capstone.Arogya.dto.RegisterRequest;
import com.capstone.Arogya.model.Role;
import com.capstone.Arogya.model.User;
import com.capstone.Arogya.repository.RoleRepository;
import com.capstone.Arogya.repository.UserRepository;
import com.capstone.Arogya.security.CustomUserDetailsService;
import com.capstone.Arogya.security.JwtUtil;
import lombok.RequiredArgsConstructor;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.core.Authentication;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import java.util.HashSet;
import java.util.Set;

@Service
@RequiredArgsConstructor
public class AuthService {

    private final UserRepository userRepository;
    private final RoleRepository roleRepository;
    private final AuthenticationManager authenticationManager;
    private final JwtUtil jwtUtil;
    private final PasswordEncoder passwordEncoder;
    private final com.capstone.Arogya.security.CustomUserDetailsService userDetailsService;

    @Transactional
    public AuthResponse register(RegisterRequest req) {
        if (userRepository.findByUsername(req.getUsername()).isPresent()) {
            throw new IllegalArgumentException("Username already taken");
        }

        // fetch role if present; otherwise create fallback
        Role userRole = roleRepository.findByName("ROLE_USER")
                .orElseGet(() -> roleRepository.save(new Role(null, "ROLE_USER", new HashSet<>())));

        User user = User.builder()
                .username(req.getUsername())
                .password(passwordEncoder.encode(req.getPassword()))
                .email(req.getEmail())
                .firstName(req.getFirstName())
                .lastName(req.getLastName())
                .build();

       
        if (user.getRoles() == null) {
            user.setRoles(new HashSet<>());
        }
        user.getRoles().add(userRole);

        User saved = userRepository.save(user);

        UserDetails ud = userDetailsService.loadUserByUsername(saved.getUsername());
        String token = jwtUtil.generateToken(ud);
        return AuthResponse.builder().token(token).build();
    }


    public AuthResponse login(AuthRequest req) {
        Authentication auth = authenticationManager.authenticate(
                new UsernamePasswordAuthenticationToken(req.getUsername(), req.getPassword())
        );

        UserDetails ud = (UserDetails) auth.getPrincipal();
        String token = jwtUtil.generateToken(ud);
        return AuthResponse.builder().token(token).build();
    }
    @Transactional(readOnly = true)
        public Long getUserId(String username) {
            return userRepository.findByUsername(username)
                    .orElseThrow(() -> new IllegalArgumentException("User not found: " + username))
                    .getId();
        }
}
