package com.capstone.Arogya.security;

import io.jsonwebtoken.*;
import io.jsonwebtoken.security.Keys;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

import java.security.Key;
import java.util.*;
import java.util.Date;
import java.util.function.Function;

@Component
public class JwtUtil {

    @Value("${jwt.secret}")
    private String secret;

    @Value("${jwt.expiration-ms}")
    private long expirationMs;

    private Key getSigningKey() {
        byte[] keyBytes = secret.getBytes();
        return Keys.hmacShaKeyFor(Arrays.copyOf(keyBytes, 32)); // ensure size; key should be long enough
    }

    public String generateToken(org.springframework.security.core.userdetails.UserDetails userDetails) {
            Map<String, Object> claims = new HashMap<>();

    // Roles
    claims.put("roles", userDetails.getAuthorities()
            .stream()
            .map(GrantedAuthority::getAuthority)
            .toList());

    // If your UserDetails is a wrapper around your User entity
    if (userDetails instanceof AppUserDetails aud) {
        var user = aud.getUser();
        claims.put("email", user.getEmail());
        claims.put("firstName", user.getFirstName());
        claims.put("lastName", user.getLastName());
        claims.put("id", user.getId());
    }
        return Jwts.builder()
                .setClaims(claims)
                .setSubject(userDetails.getUsername())
                .setIssuedAt(new Date())
                .setExpiration(new Date(System.currentTimeMillis() + expirationMs))
                .signWith(getSigningKey(), SignatureAlgorithm.HS256)
                .compact();
    }

    public String extractUsername(String token) {
        return extractClaim(token, Claims::getSubject);
    }

    private <T> T extractClaim(String token, Function<Claims, T> resolver) {
        Claims claims = Jwts.parserBuilder()
                .setSigningKey(getSigningKey())
                .build()
                .parseClaimsJws(token)
                .getBody();
        return resolver.apply(claims);
    }

    public boolean validateToken(String token, org.springframework.security.core.userdetails.UserDetails userDetails) {
        final String username = extractUsername(token);
        return username.equals(userDetails.getUsername())
                && !isTokenExpired(token);
    }

    private boolean isTokenExpired(String token) {
        Date exp = extractClaim(token, Claims::getExpiration);
        return exp.before(new Date());
    }
}
